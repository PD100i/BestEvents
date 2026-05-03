using BestEvents.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace BestEvents
{
    /// <summary>
    /// Реализация сервиса бронирования
    /// </summary>
    public class BookingService(AppDbContext db, EntityMapper mapper) : IBookingService
    {
        /// <inheritdoc/>
        public static TimeSpan AllowedTimeUntilEventEnd { get; set; } = TimeSpan.FromHours(2);

        /// <inheritdoc/>
        public async Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var bookingEntity = await db.Bookings.FindAsync(bookingId, ct);
            if (bookingEntity == null)
                throw new BookingNotFoundException(string.Format(Messages_ru.BookingNotFound, bookingId));
            return mapper.MapEntityToBooking(bookingEntity);
        }

        /// <inheritdoc/>
        public async Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            await using var transaction = await db.Database.BeginTransactionAsync(ct);

            var eventEntity = await db.Events.FromSqlRaw(
            "SELECT * FROM events WHERE id = {0} FOR UPDATE", eventId)
            .FirstOrDefaultAsync();

            if (eventEntity == null)
                throw new EventNotFoundException(string.Format(Messages_ru.CreateBookingEventNotFound, eventId));

            Event _event = mapper.MapEntityToEvent(eventEntity);
            if (_event.EndAt < DateTime.Now)
                throw new EventCompletedException();
            if (!_event.TryReserveSeats())
                throw new NoAvailableSeatsException();

            var booking = new Booking(Guid.NewGuid());
            await db.Bookings.AddAsync(mapper.MapBookingToEntity(booking), ct);

            mapper.UpdateEventEntity(_event, eventEntity);
            db.Events.Update(eventEntity);

            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return booking;
        }

        /// <inheritdoc/>
        public List<Guid> GetPendingBookings()
        {
            return db.Bookings
                .Where(b => b.Status == BookingStatus.Pending)
                .Select(b => b.Id)
                .ToList();
        }

        /// <inheritdoc/>
        public async Task TryProcessBooking(Guid id, CancellationToken ct)
        {
            EventEntity? eventEntity = null;
            Booking? booking = null;
            BookingEntity? bookingEntity = null;

            ct.ThrowIfCancellationRequested();
            await using var transaction = await db.Database.BeginTransactionAsync(ct);

            try
            {
                bookingEntity = await db.Bookings.FromSqlRaw(
                "SELECT * FROM bookins WHERE id = {0} FOR UPDATE", id)
                .FirstOrDefaultAsync();

                if (bookingEntity == null)
                    throw new BookingNotFoundException(string.Format(Messages_ru.BookingNotFound, id));

                eventEntity = await db.Events.FromSqlRaw(
                "SELECT * FROM events WHERE id = {0} FOR UPDATE", bookingEntity.EventId)
                .FirstOrDefaultAsync();

                if (eventEntity == null)
                    throw new EventNotFoundException(string.Format(Messages_ru.CreateBookingEventNotFound, bookingEntity.EventId));

                if (eventEntity.EndAt < DateTime.Now)
                    throw new EventCompletedException();

                booking = mapper.MapEntityToBooking(bookingEntity);
                booking.Confirm();


            }
            catch
            {
                if (eventEntity != null)
                {
                    Event _event = mapper.MapEntityToEvent(eventEntity);
                    _event.ReleaseSeats();
                    mapper.UpdateEventEntity(_event, eventEntity);
                    db.Events.Update(eventEntity);
                }
                if (booking != null)
                {
                    booking.Reject();
                    db.Bookings.Update(mapper.MapBookingToEntity(booking));
                }
                throw;
            }
            finally
            {
                await db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
        }
    }
}

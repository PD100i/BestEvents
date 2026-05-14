using BestEvents.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Threading;

namespace BestEvents
{
    /// <summary>
    /// Реализация сервиса бронирования
    /// </summary>
    public class BookingRepository(AppDbContext db, EntityMapper mapper) : IBookingRepository
    {
        /// <inheritdoc/>
        public static TimeSpan AllowedTimeUntilEventEnd { get; set; } = TimeSpan.FromHours(2);

        /// <inheritdoc/>
        public async Task<Booking> GetBookingAsync(Guid bookingId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var bookingEntity = await db.Bookings.Include(b => b.Event).FirstOrDefaultAsync(b => b.Id == bookingId, ct);
            if (bookingEntity == null)
                throw new BookingNotFoundException(string.Format(Messages_ru.BookingNotFound, bookingId));
            return mapper.MapEntityToBooking(bookingEntity);
        }

        /// <inheritdoc/>
        public async Task<Booking> AddBookingAsync(Booking booking, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            
            var bookingEntity = mapper.MapBookingToEntity(booking);
            await db.Bookings.AddAsync(bookingEntity, ct);
            await db.SaveChangesAsync(ct);

            return booking;
            
            
        }

        /// <inheritdoc/>
        public async Task<Booking> AddBookingAsync(Guid eventId, Func<Event, CancellationToken, Task<Booking>> AddBookingAction, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            await using var transaction = db.Database.CurrentTransaction ?? await db.Database.BeginTransactionAsync(ct);

            try
            {
                var eventEntity = await db.Events.FromSql(
                $"SELECT * FROM events WHERE id = {eventId} FOR UPDATE")
                .FirstOrDefaultAsync(ct);

                if (eventEntity == null)
                    throw new EventNotFoundException(string.Format(Messages_ru.CreateBookingEventNotFound, eventId));

                Event _event = mapper.MapEntityToEvent(eventEntity);

                var booking =  await AddBookingAction(_event, ct);

                await AddBookingAsync(booking, ct);

                mapper.UpdateEventEntity(_event, eventEntity);
                db.Events.Update(eventEntity);

                await db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return booking;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }

        }


        /// <inheritdoc/>
        public List<Guid> GetPendingBookings()
        {
            return [.. db.Bookings
                .Where(b => b.Status == BookingStatus.Pending)
                .Select(b => b.Id)];
        }


        /// <inheritdoc/>
        public async Task UpdateBookingAsync(Booking booking, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await using var transaction = await db.Database.BeginTransactionAsync(ct);

            try
            {
                var bookingEntity = await db.Bookings.FromSqlRaw(
                    "SELECT * FROM bookings WHERE id = {0} FOR UPDATE", booking.Id)
                    .FirstOrDefaultAsync();

                if (bookingEntity == null)
                    throw new BookingNotFoundException(string.Format(Messages_ru.BookingNotFound, booking.Id));

                var eventEntity = await db.Events.FromSqlRaw(
                    "SELECT * FROM events WHERE id = {0} FOR UPDATE", bookingEntity.EventId)
                    .FirstOrDefaultAsync();

                if (eventEntity == null)
                    throw new EventNotFoundException(string.Format(Messages_ru.CreateBookingEventNotFound, bookingEntity.Id));

                bookingEntity.Event = eventEntity;

                mapper.UpdateBookingEntity(booking, bookingEntity);
                db.Bookings.Update(bookingEntity);
                await db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

    }
}

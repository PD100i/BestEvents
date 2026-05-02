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
            "SELECT * FROM products WHERE id = {0} FOR UPDATE", eventId)
            .FirstOrDefaultAsync();

            if (eventEntity == null)
                throw new EventNotFoundException(string.Format(Messages_ru.CreateBookingEventNotFound, eventId));

            Event _event = mapper.MapEntityToEvent(eventEntity);
            if (_event.EndAt < DateTime.Now)
                throw new EventCompletedException();
            if (!_event.TryReserveSeats())
                throw new NoAvailableSeatsException();

            var booking = new Booking(Guid.NewGuid());

            mapper.UpdateEventEntity(_event, eventEntity);
            await db.Bookings.AddAsync(mapper.MapBookingToEntity(booking), ct);
            await db.SaveChangesAsync(ct);

            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return booking;
        }
    }
}

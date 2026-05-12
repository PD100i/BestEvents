using BestEvents.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        public List<Guid> GetPendingBookings()
        {
            return db.Bookings
                .Where(b => b.Status == BookingStatus.Pending)
                .Select(b => b.Id)
                .ToList();
        }

        /// <inheritdoc/>
        public async Task UpdateBooking(Guid id, Func<Booking, Task> action, CancellationToken ct)
        {
            EventEntity? eventEntity = null;
            Booking? booking = null;
            BookingEntity? bookingEntity = null;

            ct.ThrowIfCancellationRequested();
            await using var transaction = await db.Database.BeginTransactionAsync(ct);

            try
            {
                bookingEntity = await db.Bookings.FromSqlRaw(
                "SELECT * FROM bookings WHERE id = {0} FOR UPDATE", id)
                .FirstOrDefaultAsync();

                if (bookingEntity == null)
                    throw new BookingNotFoundException(string.Format(Messages_ru.BookingNotFound, id));

                eventEntity = await db.Events.FromSqlRaw(
                "SELECT * FROM events WHERE id = {0} FOR UPDATE", bookingEntity.EventId)
                .FirstOrDefaultAsync();

                bookingEntity.Event = eventEntity;

                booking = mapper.MapEntityToBooking(bookingEntity);

                await action(booking);

                mapper.UpdateBookingEntity(booking, bookingEntity);
                db.Bookings.Update(bookingEntity);
            }
            
            finally
            {
                await db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
        }
    }
}

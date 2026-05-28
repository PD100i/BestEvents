using Microsoft.EntityFrameworkCore;
using BestEvents.Domain;
using BestEvents.Application;
using BestEvents.Infrastructure.Exceptions;

namespace BestEvents.Infrastructure
{
    /// <summary>
    /// Реализация сервиса бронирования
    /// </summary>
    public class BookingRepository(AppDbContext db, EntityMapper mapper) : IBookingRepository
    {

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
        public async Task AddBookingAsync(Booking booking, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var bookingEntity = mapper.MapBookingToEntity(booking);
            await db.Bookings.AddAsync(bookingEntity, ct);
            await db.SaveChangesAsync(ct);
        }

        /// <inheritdoc/>
        public async Task<Booking> GetBookingForUpdateAsync(Guid bookingId, CancellationToken ct)
        {
            int operationTimeout = 2000;

            ct.ThrowIfCancellationRequested();

            await db.Database.ExecuteSqlRawAsync("SELECT set_config('lock_timeout', {0}, true);", operationTimeout.ToString());

            var bookingEntity = await db.Bookings.FromSqlRaw(
                    "SELECT * FROM bookings WHERE id = {0} FOR UPDATE", bookingId)
                    .FirstOrDefaultAsync(ct);

            if (bookingEntity == null)
                throw new BookingNotFoundException(string.Format(Messages_ru.BookingNotFound, bookingId));

            var eventEntity = await db.Events.FromSqlRaw(
                "SELECT * FROM events WHERE id = {0} FOR UPDATE", bookingEntity.EventId)
                .FirstOrDefaultAsync(ct);
            if (eventEntity == null)
                throw new EventNotFoundException(string.Format(Messages_ru.EventNotFound, bookingEntity.EventId));

            bookingEntity.Event = eventEntity;

            return mapper.MapEntityToBooking(bookingEntity);
        }

        /// <inheritdoc/>
        public async Task<List<Guid>> GetPendingBookingsAsync(CancellationToken ct)
        {
            return await db.Bookings
                .Where(b => b.Status == BookingStatus.Pending)
                .Select(b => b.Id).ToListAsync(ct);
        }


        /// <inheritdoc/>
        public async Task UpdateBookingAsync(Booking booking, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                if (booking.Event != null)
                {
                    var existingEvent = await db.Events.FirstAsync(e => e.Id == booking.Event.Id);
                    if (existingEvent == null)
                        throw new EventNotFoundException(Messages_ru.EventNotFound);
                    mapper.UpdateEventEntity(booking.Event, existingEvent);
                }
                var bookingEntity = await db.Bookings.FirstAsync(b => b.Id == booking.Id, ct);
                mapper.UpdateBookingEntity(booking, bookingEntity);
                await db.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                throw new UpdateBookingException(string.Format(Messages_ru.UpdateBookingErrorMessage, booking.Id), ex);
            }
        }
    }
}

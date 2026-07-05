using Microsoft.EntityFrameworkCore;
using Bookings.Domain;
using Bookings.Application;
using Bookings.Infrastructure.Exceptions;
using Common;

namespace Bookings.Infrastructure
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
            var bookingEntity = await db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, ct);
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
                var bookingEntity = await db.Bookings.FirstAsync(b => b.Id == booking.Id, ct);
                mapper.UpdateBookingEntity(booking, bookingEntity);
            }
            catch (Exception ex)
            {
                throw new UpdateBookingException(string.Format(Messages_ru.UpdateBookingErrorMessage, booking.Id), ex);
            }
        }

        /// <inheritdoc/>
        public async Task<List<Booking>> GetActiveBookingsByUserAsync(Guid userId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var bookingEntities = await db.Bookings
                .Where(b => b.UserId == userId && (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed))
                .ToListAsync(ct);
            return bookingEntities.Select(mapper.MapEntityToBooking).ToList();
        }

        /// <inheritdoc/>
        public async Task EnqueueBookingCreatedAsync(BookingCreatedMessage message, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await db.BookingCreatedOutbox.AddAsync(mapper.MapBookingCreatedMessageToEntity(message), ct);
        }

        /// <inheritdoc/>
        public async Task DequeueBookingCreatedAsync(Guid bookingId, CancellationToken ct = default)
        {
            await db.BookingCreatedOutbox
                .Where(m => m.BookingId == bookingId)
                .ExecuteDeleteAsync(ct);
        }

        /// <inheritdoc/>
        public async Task<BookingCreatedMessage?> GetUnpublishedCreatedBookingAsync(CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var message = await db.BookingCreatedOutbox
                .OrderBy(m => m.CreatedAt)
                .FirstOrDefaultAsync(ct);
            if (message == null)
                return null;
            return mapper.MapBookingMessageEntityToMessage(message);
        }
    }
}

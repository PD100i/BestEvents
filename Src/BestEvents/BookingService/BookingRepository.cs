using BestEvents.Exceptions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace BestEvents
{
    /// <inheritdoc/>
    public class BookingRepository: IBookingRepository
    {
        static readonly ConcurrentDictionary<Guid, Booking> bookingRepo = new();

        /// <inheritdoc/>
        public async Task<Booking> CreateBookingAsync(Booking booking, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return await Task.Run(() =>
            {
                bookingRepo.TryAdd(booking.Id, booking);
                return booking;
            });
        }

        /// <inheritdoc/>
        public async Task<Booking> GetBookingAsync(Guid id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await Task.Run(() =>
            {
                if (!bookingRepo.TryGetValue(id, out Booking? value))
                    throw new DataNotFoundException($"Бронеирование с идентификатором {id} не найдено");
                return value;
            });
        }

        /// <inheritdoc/>
        public async Task<List<Booking>> GetPendingBookingAsync(CancellationToken ct)
        {
           return await Task.FromResult(bookingRepo.Values.Where(b => b.Status == BookingStatus.Pending)
                                                          .OrderBy(b => b.ProcessedAt) 
                                                          .ToList());
        }

        /// <inheritdoc/>
        public async Task ReplaceBookingAsync(Booking booking, CancellationToken ct)
        {
            if (!bookingRepo.ContainsKey(booking.Id))
                throw new DataNotFoundException($"Бронирование с идентификатором {booking.Id} не найдено. Обновление не произведено");
            await Task.Run(() => bookingRepo[booking.Id] = booking);
        }
    }
}

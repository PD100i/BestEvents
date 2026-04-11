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
            bookingRepo.TryAdd(booking.Id, booking);
            return await Task.FromResult(booking);
        }

        /// <inheritdoc/>
        public async Task<Booking> GetBookingAsync(Guid id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            if (!bookingRepo.TryGetValue(id, out Booking? value))
                throw new BookingNotFoundException(string.Format(Messages_ru.BookingNotFound, id));
            return await Task.FromResult(value);
        }

        /// <inheritdoc/>
        public async Task<List<Booking>> GetPendingBookingAsync(CancellationToken ct)
        {
           return await Task.FromResult(bookingRepo.Values.Where(b => b.Status == BookingStatus.Pending)
                                                          .OrderBy(b => b.ProcessedAt) 
                                                          .ToList());
        }

        /// <inheritdoc/>
        public Task ReplaceBookingAsync(Booking booking, CancellationToken ct)
        {
            if (!bookingRepo.ContainsKey(booking.Id))
                throw new BookingNotFoundException(string.Format(Messages_ru.BookingNotReplaced, booking.Id));
            bookingRepo[booking.Id] = booking;
            return Task.CompletedTask; 
        }
    }
}

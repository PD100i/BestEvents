using BestEvents.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace BestEvents
{
    /// <summary>
    /// Реализация сервиса бронирования
    /// </summary>
    public class BookingService(IBookingRepository bookingRepository) : IBookingService
    {
        
        /// <inheritdoc/>
        public async Task<Booking> GetBookingAsync(Guid bookingId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await bookingRepository.GetBookingAsync(bookingId, ct);
        }

        /// <inheritdoc/>
        public async Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var bookingId = Guid.NewGuid();
            var booking = await bookingRepository.AddBookingAsync(bookingId, eventId, Booking.CreateBooking, ct);
            return booking;
        }

        
        /// <inheritdoc/>
        public async Task<List<Guid>> GetPendingBookingsAsync(CancellationToken ct)
        {
            return await bookingRepository.GetPendingBookingsAsync(ct);
        }

        /// <inheritdoc/>
        public async Task TryProcessBooking(Guid bookingId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            try
            {
                await bookingRepository.UpdateBookingAsync(bookingId, Booking.Confirm, ct);
            }
            catch (BookingDoubleProcessingException)
            {
                throw;
            }
            catch (BookingNotFoundException)
            {
                throw;
            }           
            catch
            {
                await bookingRepository.UpdateBookingAsync(bookingId, Booking.Reject, ct);
                throw;
            }
        }
    }
}

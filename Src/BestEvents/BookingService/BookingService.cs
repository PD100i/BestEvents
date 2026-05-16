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
            var booking = await bookingRepository.AddBookingAsync(eventId, CreateBookingAction, ct);
            return booking;
        }

        /// <summary>
        /// Создает экземпляр бронирования на событие _event или бросает исключение, если бронирование невозможно
        /// </summary>
        /// <param name="_event"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="EventCompletedException"></exception>
        /// <exception cref="NoAvailableSeatsException"></exception>
        public static async Task<Booking> CreateBookingAction(Event _event, CancellationToken ct)
        {
            if (_event.EndAt < DateTime.UtcNow)
                throw new EventCompletedException();

            if (!_event.TryReserveSeats())
                throw new NoAvailableSeatsException();

            Guid bookingId = Guid.NewGuid();
            return await Task.FromResult(new Booking(bookingId, _event));

        }

        /// <inheritdoc/>
        public List<Guid> GetPendingBookings()
        {
            return bookingRepository.GetPendingBookings();
        }

        /// <inheritdoc/>
        public async Task TryProcessBooking(Guid bookingId, CancellationToken ct)
        {
            Booking? booking = null;

            ct.ThrowIfCancellationRequested();

            try
            {
                booking = await bookingRepository.GetBookingAsync(bookingId, ct);

                if (booking.Event == null)
                    throw new EventNotFoundException(string.Format(Messages_ru.CreateBookingEventNotFound, booking.EventId));

                if (booking.Event.EndAt < DateTime.UtcNow)
                    throw new EventCompletedException();
                booking.Confirm();
                await bookingRepository.UpdateBookingAsync(booking, ct);

            }
            catch (BookingDoubleProcessingException)
            {
                throw;
            }
            catch (BookingNotFoundException)
            {
                throw;
            }
            catch (EventNotFoundException)
            {
                if (booking != null)
                {
                    booking.Reject();
                    await bookingRepository.UpdateBookingAsync(booking, ct);
                }
                throw;
            }
            catch
            {
                if (booking != null)
                {
                    booking.Reject();
                    booking.Event?.ReleaseSeats();
                    await bookingRepository.UpdateBookingAsync(booking, ct);
                }
                throw;
            }
        }
    }
}

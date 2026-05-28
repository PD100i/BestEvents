using BestEvents.Domain;
using BestEvents.Domain.Exceptions;
using BestEvents.Application.Exceptions;


namespace BestEvents.Application
{
    /// <summary>
    /// Реализация сервиса бронирования
    /// </summary>
    public class BookingService(IBookingRepository bookingRepository, IEventRepository eventRepository, IUnitOfWork uow) : IBookingService
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
            using var transaction = await uow.BeginTransactionAsync();
            var _event = await eventRepository.GetEventForUpdateAsync(eventId, ct);
            if (_event.EndAt < DateTime.UtcNow)
                throw new EventCompletedException();
            if (!_event.TryReserveSeats())
                throw new NoAvailableSeatsException();

            var booking = new Booking(bookingId, _event);
            await eventRepository.ReplaceEventAsync(_event, ct);
            await bookingRepository.AddBookingAsync(booking, ct);
            await transaction.CommitAsync(ct);
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
            using var transaction = await uow.BeginTransactionAsync();
            Booking? booking = null;
            try
            {               
                booking = await bookingRepository.GetBookingForUpdateAsync(bookingId, ct);
                if (booking.Event == null)
                    throw new EventNotExistsException(string.Format(Messages_ru.CreateBookingEventNotFound, booking.EventId));
                if (booking.Event.EndAt < DateTime.UtcNow)
                    throw new EventCompletedException();
                booking.Confirm();
                await bookingRepository.UpdateBookingAsync(booking, ct);
                await transaction.CommitAsync(ct);
            }
            catch (BookingDoubleProcessingException)
            {
                throw;
            }         
            catch
            {
                if (booking == null) 
                    throw;
                booking.Reject();
                if (booking.Event != null)
                    booking.Event.ReleaseSeats();
                await bookingRepository.UpdateBookingAsync(booking, ct);
                await transaction.CommitAsync(ct);
                throw;
            }
        }
    }
}

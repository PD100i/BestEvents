using BestEvents.Domain;
using BestEvents.Domain.Exceptions;
using BestEvents.Application.Exceptions;


namespace BestEvents.Application
{
    /// <summary>
    /// Реализация сервиса бронирования
    /// </summary>
    public class BookingService(IBookingRepository bookingRepository, IEventRepository eventRepository, IUserRepository userRepository, IUnitOfWork uow, IUserAccessor userAccessor) : IBookingService
    {
        const int MaxBookingsPerUser = 10;

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
            using var transaction = await uow.BeginTransactionAsync();
            var bookingId = Guid.NewGuid();
            var _event = await eventRepository.GetEventForUpdateAsync(eventId, ct);
            if (_event.EndAt < DateTime.UtcNow)
                throw new CreateBookingException(Messages_ru.EventCompleted);
            if (_event.StartAt < DateTime.UtcNow)
                throw new CreateBookingException(Messages_ru.EventBegun);

            // Получаем информацию о пользователе из контекста запроса
            var userFromRequest = userAccessor.GetUser();

            // Проверяем, что пользователь не превысил лимит активных бронирований
            await CheckUsersBookingAvailability(userFromRequest.Id, ct);

            string userName = userFromRequest.Name;
            var user = await userRepository.GetUserAsync(userName, ct);
            if (user == null)
                throw new NoRightForOperation(string.Format(Messages_ru.UserNotFound, userName));

            // Проверяем наличие свободных мест и резервируем их
            _event.TryReserveSeats();

            var booking = new Booking(bookingId, _event, user);
            await eventRepository.ReplaceEventAsync(_event, ct);
            await bookingRepository.AddBookingAsync(booking, ct);
            await transaction.CommitAsync(ct);
            return booking;
        }

        /// <inheritdoc/>
        public async Task CancelBookingAsync(Guid bookingId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            using var transaction = await uow.BeginTransactionAsync();
            var booking = await bookingRepository.GetBookingForUpdateAsync(bookingId, ct);
            var user = userAccessor.GetUser();
            booking.Cancel(user);
            if (booking.Event != null)
                booking.Event.ReleaseSeats();
            await bookingRepository.UpdateBookingAsync(booking, ct);
            await transaction.CommitAsync(ct);
        }

        /// <inheritdoc/>
        public async Task<List<Guid>> GetPendingBookingsAsync(CancellationToken ct)
        {
            using var transaction = await uow.BeginTransactionAsync();
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
                    throw new BookingProcessException(Messages_ru.EventCompleted);
                if (booking.Event.StartAt < DateTime.UtcNow)
                    throw new BookingProcessException(Messages_ru.EventBegun);
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

        private async Task CheckUsersBookingAvailability(Guid userId, CancellationToken ct)
        {
            var bookings = await bookingRepository.GetActiveBookingsByUserAsync(userId, ct);
            if (bookings == null)
                return;
            if (bookings.Count >= MaxBookingsPerUser)
                throw new BookingLimitExceededException(string.Format(Messages_ru.BookingLimitExceeded, MaxBookingsPerUser));
        }
    }
}

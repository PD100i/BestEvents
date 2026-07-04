using Bookings.Domain;
using Bookings.Application.Exceptions;


namespace Bookings.Application
{
    /// <summary>
    /// Реализация сервиса бронирования
    /// </summary>
    public class BookingService(IBookingRepository bookingRepository,  IUserAccessor userAccessor) : IBookingService
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
           
            var bookingId = Guid.NewGuid();
          
             // Получаем информацию о пользователе из контекста запроса
            var user = userAccessor.GetUser();

            // Проверяем, что пользователь не превысил лимит активных бронирований
            await CheckUsersBookingAvailability(user.Id, ct);

            var booking = new Booking(bookingId, eventId, user.Id);           
            await bookingRepository.AddBookingAsync(booking, ct);
            
            //ЗДЕСЬ НУЖНО ОПУБЛИКОВАТЬ СОБЫТИЕ

            return booking;
        }

        
        /// <inheritdoc/>
        public async Task<List<Guid>> GetPendingBookingsAsync(CancellationToken ct)
        {
            return await bookingRepository.GetPendingBookingsAsync(ct);
        }

        ///// <inheritdoc/>
        //public async Task TryProcessBooking(Guid bookingId, CancellationToken ct)
        //{
        //    ct.ThrowIfCancellationRequested();
        //    using var transaction = await uow.BeginTransactionAsync();
        //    Booking? booking = null;
        //    try
        //    {               
        //        booking = await bookingRepository.GetBookingForUpdateAsync(bookingId, ct);
        //        if (booking.Event == null)
        //            throw new EventNotExistsException(string.Format(Messages_ru.CreateBookingEventNotFound, booking.EventId));
        //        if (booking.Event.EndAt < DateTime.UtcNow)
        //            throw new BookingProcessException(Messages_ru.EventCompleted);
        //        if (booking.Event.StartAt < DateTime.UtcNow)
        //            throw new BookingProcessException(Messages_ru.EventBegun);
        //        booking.Confirm();
        //        await bookingRepository.UpdateBookingAsync(booking, ct);
        //        await transaction.CommitAsync(ct);
        //    }
        //    catch (BookingDoubleProcessingException)
        //    {
        //        throw;
        //    }         
        //    catch
        //    {
        //        if (booking == null) 
        //            throw;
        //        booking.Reject();
        //        if (booking.Event != null)
        //            booking.Event.ReleaseSeats();
        //        await bookingRepository.UpdateBookingAsync(booking, ct);
        //        await transaction.CommitAsync(ct);
        //        throw;
        //    }
            
        //}

        private async Task CheckUsersBookingAvailability(Guid userId, CancellationToken ct)
        {
            var bookings = await bookingRepository.GetActiveBookingsByUserAsync(userId, ct);
            if (bookings == null)
                return;
            if (bookings.Count >= MaxBookingsPerUser)
                throw new BookingLimitExceededException(string.Format(Messages_ru.BookingLimitExceeded, MaxBookingsPerUser));
        }

        public async Task CancelBookingAsync(Guid id, User user, CancellationToken ct)
        {
            Booking booking = await bookingRepository.GetBookingAsync(id, ct);
            booking.Cancel(user);
        }
    }
}

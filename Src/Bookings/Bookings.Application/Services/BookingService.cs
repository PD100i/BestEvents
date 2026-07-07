using Bookings.Application.Exceptions;
using Bookings.Domain;
using Bookings.Domain.Exceptions;
using Common;
using Microsoft.Extensions.Logging;

namespace Bookings.Application
{
    /// <summary>
    /// Реализация сервиса бронирования
    /// </summary>
    public class BookingService(IBookingRepository repository,  IUserAccessor userAccessor, IUnitOfWork uow) : IBookingService
    {
        const int MaxBookingsPerUser = 10;
        /// <inheritdoc/>
        public async Task<Booking> GetBookingAsync(Guid bookingId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return await repository.GetBookingAsync(bookingId, ct);
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
            await repository.AddBookingAsync(booking, ct);
            var message = new BookingMessage()
            {
                BookingId = bookingId,
                EventId = eventId,
                CreatedAt = DateTime.UtcNow,
            };
            await repository.EnqueueBookingCreatedAsync(message, ct);
            await uow.SaveChangesAsync();
            return booking;
        }


        public async Task ConfirmBooking(Guid bookingId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            using var transaction = await uow.BeginTransactionAsync();
            Booking? booking = null;
            booking = await repository.GetBookingForUpdateAsync(bookingId, ct);
            booking.Confirm();
            await repository.UpdateBookingAsync(booking, ct);
            await uow.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }

        public async Task RejectedBooking(Guid bookingId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            using var transaction = await uow.BeginTransactionAsync();
            Booking? booking = null;
            booking = await repository.GetBookingForUpdateAsync(bookingId, ct);
            booking.Reject();
            await repository.UpdateBookingAsync(booking, ct);
            await uow.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }

        public async Task CancelBookingAsync(Guid id, CancellationToken ct)
        {
            using var transaction = await uow.BeginTransactionAsync();
            var booking = await repository.GetBookingForUpdateAsync(id, ct);
            var user = userAccessor.GetUser();
            booking.Cancel(user);
            await repository.UpdateBookingAsync(booking, ct);
            var message = new BookingMessage()
            {
                BookingId = booking.Id,
                EventId = booking.EventId,
                CreatedAt = DateTime.UtcNow,
            };
            await repository.EnqueueBookingCancelledAsync(message, ct);
            await uow.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }

        private async Task CheckUsersBookingAvailability(Guid userId, CancellationToken ct)
        {
            var bookings = await repository.GetActiveBookingsByUserAsync(userId, ct);
            if (bookings == null)
                return;
            if (bookings.Count >= MaxBookingsPerUser)
                throw new BookingLimitExceededException(string.Format(Messages_ru.BookingLimitExceeded, MaxBookingsPerUser));
        }

       
    }
}

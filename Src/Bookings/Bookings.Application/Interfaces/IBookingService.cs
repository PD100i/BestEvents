using Bookings.Domain;
using Common;

namespace Bookings.Application
{
    /// <summary>
    /// Интерфейс сервиса бронирований
    /// </summary>
    public interface IBookingService
    {
       
        /// <summary>
        /// Бронирование события по его идентификатору.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken ct);

        /// <summary>
        /// Получение бронирования по его идентификатору. Метод возвращает результат бронирования
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> GetBookingAsync(Guid bookingId, CancellationToken ct);

        /// <summary>
        /// Подтверждает бронирование. Изменяет статус бронирования на Confirmed
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        Task ConfirmBooking(Guid bookingId, CancellationToken ct);

        /// <summary>
        /// Jnrkjyztn бронирование. Изменяет статус бронирования на Rejected
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        Task RejectedBooking(Guid bookingId, CancellationToken ct);

        /// <summary>
        /// Отменяет бронирование. Изменяет статус бронирования на Cancelled
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task CancelBookingAsync(Guid id, CancellationToken ct);

        
    }
}

using Bookings.Domain;

namespace Bookings.Application
{
    /// <summary>
    /// Интерфейс сервиса бронирований
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Минимальное время до конца события, когда еще разрешено бронирование.
        /// </summary>
        static TimeSpan AllowedTimeUntilEventEnd { get; set; }

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
        /// Возвращает список бронирований, ожидающих обработки
        /// </summary>
        /// <returns></returns>
        Task<List<Guid>> GetPendingBookingsAsync(CancellationToken ct);

        /// <summary>
        /// Отменяет бронирование. При успешной изменяет статус бронирования на Cancelled и освобождает зарезервированные места на событии
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task CancelBookingAsync(Guid id, CancellationToken ct);
    }
}

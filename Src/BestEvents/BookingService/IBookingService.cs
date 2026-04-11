namespace BestEvents
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
        Task<BookingResultDto> CreateBookingAsync(string eventId, CancellationToken ct);

        /// <summary>
        /// Получение бронирования по его идентификатору. Метод возвращает результат бронирования
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<BookingResultDto> GetBookingByIdAsync(string bookingId, CancellationToken ct);
    }
}

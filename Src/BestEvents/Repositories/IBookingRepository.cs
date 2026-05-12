namespace BestEvents
{
    /// <summary>
    /// Интерфейс репозитория бронирований
    /// </summary>
    public interface IBookingRepository
    {

        /// <summary>
        /// Создает новое бронирование  .
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> AddBookingAsync(Booking booking, CancellationToken ct);

        /// <summary>
        /// Получение бронирования по его идентификатору. Метод возвращает результат бронирования
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> GetBookingAsync(Guid bookingId, CancellationToken ct);

        /// <summary>
        /// Читает бронирование по id, затем обновляет согласно переданному методу action
        /// </summary>
        /// <param name="id"></param>
        /// <param name="action"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> UpdateBookingAsync(Guid id, Func<Booking, Task> action, CancellationToken ct = default);

        /// <summary>
        /// Возвращает список id необработанных броней
        /// </summary>
        /// <returns></returns>
        List<Guid> GetPendingBookings();

        
    }
}

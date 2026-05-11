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
        Task<Booking> CreateBookingAsync(Booking booking, CancellationToken ct);

        /// <summary>
        /// Получение бронирования по его идентификатору. Метод возвращает результат бронирования
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> GetBookingAsync(Guid bookingId, CancellationToken ct);


        /// <summary>
        /// Получение бронирования по его идентификатору для обновления. 
        /// Метод должен блокировать запись до завершения транзакции, чтобы избежать гонок при обработке бронирования
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> GetBookingForUpdateAsync(Guid bookingId, CancellationToken ct);

        /// <summary>
        /// Обновляет бронирование. Метод должен сохранять изменения в базе данных и разблокировать запись, чтобы другие транзакции могли получить доступ к ней
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> UpdateBookingAsync(Booking booking, CancellationToken ct);

        /// <summary>
        /// Возвращает список бронирований, ожидающих обработки
        /// </summary>
        /// <returns></returns>
        List<Guid> GetPendingBookings();

        
    }
}

namespace BestEvents
{
    /// <summary>
    /// Интерфейс репозитория бронирований
    /// </summary>
    public interface IBookingRepository
    {
        /// <summary>
        /// Асинхронно добавляет бронирование в репозиторий.
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> AddBooking(Booking booking, CancellationToken ct);

        /// <summary>
        /// Получает бронирование по его идентификатору. Если бронирование с таким идентификатором не найдено, бросает исключение.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> GetBookingAsync(Guid id, CancellationToken ct);

        /// <summary>
        /// Возвращает асинхронно список список бронирований в ожидании обработки
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<List<Booking>> GetPendingBooking(CancellationToken ct);

        /// <summary>
        /// Перезаписывает асинхронно бронирование с таким же ID 
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task ReplaceBooking(Booking booking, CancellationToken ct);
    }
}

namespace BestEvents
{
    /// <summary>
    /// Интерфейс репозитория бронирований
    /// </summary>
    public interface IBookingRepository
    {

        /// <summary>
        /// Создает новое бронирование и добавляет в базу.
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> AddBookingAsync(Booking booking, CancellationToken ct);

        /// <summary>
        /// Создает новое бронирование, применяя метод бизнес логики action и добавляет в базу
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="AddBookingAction"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> AddBookingAsync(Guid eventId, Func<Event, CancellationToken, Task<Booking>> AddBookingAction, CancellationToken ct);

        /// <summary>
        /// Получение бронирования по его идентификатору. Метод возвращает результат бронирования
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> GetBookingAsync(Guid bookingId, CancellationToken ct);

        /// <summary>
        /// Обновляет бронирование
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task UpdateBookingAsync(Booking booking, CancellationToken ct = default);

        /// <summary>
        /// Возвращает список id необработанных броней
        /// </summary>
        /// <returns></returns>
        List<Guid> GetPendingBookings();
    }
}

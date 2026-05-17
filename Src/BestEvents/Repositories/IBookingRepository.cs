namespace BestEvents
{
    /// <summary>
    /// Интерфейс репозитория бронирований
    /// </summary>
    public interface IBookingRepository
    {
        /// <summary>
        /// Создает новое бронирование, применяя метод бизнес логики action и добавляет в базу
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="eventId"></param>
        /// <param name="CreateBookingAction">Фабричный метод создания бронирования - принимает id бронирования и событие</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> AddBookingAsync(Guid bookingId, Guid eventId, Func<Guid, Event, Booking> CreateBookingAction, CancellationToken ct);

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
        /// <param name="bookingId"></param>
        /// <param name="updateAction">Метод обновления бронирования</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task UpdateBookingAsync(Guid bookingId, Action<Booking> updateAction, CancellationToken ct = default);

        /// <summary>
        /// Возвращает список id необработанных броней
        /// </summary>
        /// <returns></returns>
        List<Guid> GetPendingBookings();
    }
}

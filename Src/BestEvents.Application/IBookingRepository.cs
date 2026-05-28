using BestEvents.Domain;

namespace BestEvents.Application
{
    /// <summary>
    /// Интерфейс репозитория бронирований
    /// </summary>
    public interface IBookingRepository
    {
        /// <summary>
        /// Создает новое бронирование, применяя метод бизнес логики action и добавляет в базу
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task AddBookingAsync(Booking booking, CancellationToken ct);

        /// <summary>
        /// Получение бронирования по его идентификатору. Метод возвращает результат бронирования
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> GetBookingAsync(Guid bookingId, CancellationToken ct);

        /// <summary>
        /// Получение бронирования по его идентификатору для последующего обновления. Блокирует строку БД
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Booking> GetBookingForUpdateAsync(Guid bookingId, CancellationToken ct);


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
        Task<List<Guid>> GetPendingBookingsAsync(CancellationToken ct = default);
    }
}

using Bookings.Domain;
using Common;

namespace Bookings.Application
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
        /// Добавляет сообщение в очередь на публикацию
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task EnqueueMessageAsync(Message message, CancellationToken ct = default);

        /// <summary>
        /// Удаляет сообщение из очереди на публикацию
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task DequeueMessageAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Возвращает самое старое неопубликованное сообщение из outbox по типу сообщения
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Message?> GetOldestUnpublishedMessageAsync(MessageTypeEnum messageType, CancellationToken ct = default);

        /// <summary>
        /// Возвращает список бронирований пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<List<Booking>> GetActiveBookingsByUserAsync(Guid userId, CancellationToken ct = default);

        

    }
}

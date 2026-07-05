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
        /// Добавляет сообщение о создании бронирования в очередь на публикацию
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task EnqueueBookingCreatedAsync(BookingCreatedMessage message, CancellationToken ct = default);

        /// <summary>
        /// Удаляет сообщение о создании бронирования из очереди на публикацию
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task DequeueBookingCreatedAsync(Guid bookingId, CancellationToken ct = default);


        /// <summary>
        /// Возвращает самое старое неопубликованное сообщение о создании брони
        /// </summary>
        /// <returns></returns>
        Task<BookingCreatedMessage?> GetUnpublishedCreatedBookingAsync(CancellationToken ct = default);

        /// <summary>
        /// Удаляет сообщение об отмене бронирования из очереди на публикацию
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task DequeueBookingCancelledAsync(Guid bookingId, CancellationToken ct = default);

        /// <summary>
        /// Возвращает самое старое неопубликованное сообщение об отмене брони
        /// </summary>
        /// <returns></returns>
        Task<BookingCreatedMessage?> GetUnpublishedCancelledBookingAsync(CancellationToken ct = default);

        /// <summary>
        /// Возвращает список бронирований пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<List<Booking>> GetActiveBookingsByUserAsync(Guid userId, CancellationToken ct = default);

        

    }
}

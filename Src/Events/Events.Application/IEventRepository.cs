using Events.Domain;
using Common;

namespace Events.Application
{
    /// <summary>
    /// Интерфейс репозитория работы с событиями
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Возвращает все события в виде списка EventDto
        /// </summary>
        /// <param name="title"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<PaginatedResult<Event>> GetEventsAsync(string? title, DateTime? from, DateTime? to, int page = 1, int size = 10, CancellationToken ct = default);

        /// <summary>
        /// Возвращает событие по его идентификатору. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Event> GetEventAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Возвращает событие по его идентификатору для последующего обновления. Блокирует строку БД 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Event> GetEventForUpdateAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Создает новое событие
        /// </summary>
        /// <param name="_event"></param>
        /// /// <param name="ct"></param>
        Task<Event> AddEventAsync(Event _event, CancellationToken ct = default);
        /// <summary>
        /// Обновляет событие. Метод должен сохранять изменения в базе данных и разблокировать запись, чтобы другие транзакции могли получить доступ к ней
        /// </summary>
        /// <param name="_event"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Event> ReplaceEventAsync(Event _event, CancellationToken ct = default);

        /// <summary>
        /// Удаляет событие по его идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        Task DeleteEventAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Добавляет в инбокс таблицу сообщение о создании бронирования
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task AddToCratedBookingInboxAsync(BookingMessage message, CancellationToken ct = default);

        /// <summary>
        /// Добавляет в инбокс таблицу сообщение об отмене бронирования
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task AddToCancelledBookingInboxAsync(BookingMessage message, CancellationToken ct = default);

        /// <summary>
        /// Добавляет сообщение о резервировании мест в очередь на публикацию
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task EnqueueSeatsReservedAsync(BookingMessage message, CancellationToken ct = default);

        /// <summary>
        /// Удаляет сообщение о резервировании мест из очереди на публикацию
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task DequeueSeatsReservedAsync(Guid bookingId, CancellationToken ct = default);

        /// <summary>
        /// Возвращает самое старое неопубликованное сообщение о резервировании мест
        /// </summary>
        /// <returns></returns>
        Task<BookingMessage?> GetUnpublishedSeatsReservedAsync(CancellationToken ct = default);


        /// <summary>
        /// Добавляет сообщение об ошибке резервирования мест в очередь на публикацию
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task EnqueueSeatsReservationErrorAsync(BookingMessage message, CancellationToken ct = default);

        /// <summary>
        /// Удаляет сообщение об ошибке резервирования мест из очереди на публикацию
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task DequeueSeatsReservationErrorAsync(Guid bookingId, CancellationToken ct = default);

        /// <summary>
        /// Возвращает самое старое неопубликованное сообщение об ошибке резервирования мест
        /// </summary>
        /// <returns></returns>
        Task<BookingMessage?> GetUnpublishedSeatsReservationErrorAsync(CancellationToken ct = default);
    }
}

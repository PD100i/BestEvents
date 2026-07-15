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
        /// Возвращает список самых популярных событий
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<List<Event>> GetTopPopularEventsAsync(CancellationToken ct = default);

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
        /// Добавляет сообщение в инбокс таблицу
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task AddMessageToInboxAsync(Message message, CancellationToken ct = default);

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
    }
}

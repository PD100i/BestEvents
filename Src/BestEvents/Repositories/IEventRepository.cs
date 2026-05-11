
namespace BestEvents
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
        /// Возвращает событие по его идентификатору. Если событие с таким идентификатором не найдено, возвращает null.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Event> GetEventAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Создает новое событие
        /// </summary>
        /// <param name="_event"></param>
        /// /// <param name="ct"></param>
        Task<Event> CreateEventAsync(Event _event, CancellationToken ct = default);

        /// <summary>
        /// Получает событие по его идентификатору для последующего обновления. Метод должен блокировать запись в БД для данного события, чтобы предотвратить одновременное обновление одной и той же записи несколькими запросами. 
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Event> GetEventForUpdateAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Обновляет событие. Метод должен сохранять изменения в базе данных и разблокировать запись, чтобы другие транзакции могли получить доступ к ней
        /// </summary>
        /// <param name="_event"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Event> UpdateEventAsync(Event _event, CancellationToken ct = default);

        /// <summary>
        /// Удаляет событие по его идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        Task DeleteEventAsync(Guid id, CancellationToken ct = default);
    }
}


namespace BestEvents
{
    /// <summary>
    /// Интерфейс сервиса работы с событиями
    /// </summary>
    public interface IEventService
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
        /// Перезаписывает событие
        /// </summary>
        /// <param name="_event"></param>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        Task ReplaceEventAsync(Guid id, Event _event, CancellationToken ct = default);

        /// <summary>
        /// Удаляет событие по его идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        Task DeleteEventAsync(Guid id, CancellationToken ct = default);
    }
}

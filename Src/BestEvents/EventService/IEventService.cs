
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
        Task<PaginatedResultDto> GetEventsAsync(string? title, DateTime? from, DateTime? to, int page = 1, int size = 10, CancellationToken ct = default);

        /// <summary>
        /// Возвращает событие по его идентификатору в виде EventDto. Если событие с таким идентификатором не найдено, возвращает null.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<EventInfoDto> GetEventAsync(string id, CancellationToken ct = default);

        /// <summary>
        /// Создает новое событие, используя данные из Dto объекта
        /// </summary>
        /// <param name="eventDto"></param>
        /// /// <param name="ct"></param>
        Task<EventInfoDto> CreateEventAsync(CreateEventDto eventDto, CancellationToken ct = default);

        /// <summary>
        /// Перезаписывает событие, используя данные из Dto объекта
        /// </summary>
        /// <param name="eventDto"></param>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        Task ReplaceEventAsync(string id, EventInfoDto eventDto, CancellationToken ct = default);

        /// <summary>
        /// Удаляет событие по его идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        Task DeleteEventAsync(string id, CancellationToken ct = default);
    }
}

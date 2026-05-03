using BestEvents.Exceptions;


namespace BestEvents
{
    /// <summary>
    /// Интерфейс репозитория для хранения и управления событиями Event.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Добавляет event в хранилище
        /// </summary>
        /// <param name="_event">Событие</param>
        /// <param name="ct"></param>
        Task<Event> AddEventAsync(Event _event, CancellationToken ct);

        /// <summary>
        /// Удаление события из хранилища по его идентификатору. Возвращает false, если не найдено
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        Task<bool> RemoveEventAsync(Guid id, CancellationToken ct);

        /// <summary>
        /// Перезапиывает событие. Возвращает false, если не найдено
        /// </summary>
        /// <param name="_event"></param>
        /// <param name="ct"></param>
        Task ReplaceEventAsync(Event _event, CancellationToken ct);

        /// <summary>
        /// Получает событие по его иденификатору. Если событие с таким идентификатором не найдено, возвращает null.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Event?> GetEventAsync(Guid id, CancellationToken ct);

        /// <summary>
        /// Получает все события из хранилища и возвращает их в виде списка.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<PaginatedResult<Event>> GetEventsAsync(string? title, DateTime? from, DateTime? to, int page = 1, int size = 10, CancellationToken ct = default);

        
    }
}

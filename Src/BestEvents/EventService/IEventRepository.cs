using BestEvents.Exceptions;

namespace BestEvents
{
    /// <summary>
    /// Интерфейс репозитория для хранения и управления событиями Event.
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Добавляет event в хранилище
        /// </summary>
        /// <param name="_event">Событие</param>
        Task<Event> AddEventAsync(Event _event);

        /// <summary>
        /// Удаление события из хранилища по его идентификатору. Возвращает false, если не найдено
        /// </summary>
        /// <param name="id"></param>
        Task<bool> RemoveEventAsync(Guid id);

        /// <summary>
        /// Перезапиывает событие. Возвращает false, если не найдено
        /// </summary>
        /// <param name="_event"></param>
        Task<bool> ReplaceEventAsync(Event _event);

        /// <summary>
        /// Получает событие по его иденификатору. Если событие с таким идентификатором не найдено, возвращает null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Event?> GetEventAsync(Guid id);

        /// <summary>
        /// Получает все события из хранилища и возвращает их в виде списка.
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<Event>> GetEventsAsync();

        
    }
}

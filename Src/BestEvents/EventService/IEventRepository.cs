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
        public Event AddEvent(Event _event);

        /// <summary>
        /// Удаление события из хранилища по его идентификатору. Возвращает false, если не найдено
        /// </summary>
        /// <param name="id"></param>
        bool RemoveEvent(Guid id);

        /// <summary>
        /// Перезапиывает событие. Возвращает false, если не найдено
        /// </summary>
        /// <param name="_event"></param>
        bool ReplaceEvent(Event _event);

        /// <summary>
        /// Получает событие по его иденификатору. Если событие с таким идентификатором не найдено, возвращает null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Event? GetEvent(Guid id);

        /// <summary>
        /// Получает все события из хранилища и возвращает их в виде списка.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Event> GetEvents();

        
    }
}

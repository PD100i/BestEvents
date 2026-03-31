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
        /// Удаление события из хранилища по его идентификатору.
        /// </summary>
        /// <param name="id"></param>
        void RemoveEvent(Guid id);

        /// <summary>
        /// Перезапиывает событие
        /// </summary>
        /// <param name="_event"></param>
        void ReplaceEvent(Event _event);

        /// <summary>
        /// Получает событие по его иденификатору. Если событие с таким идентификатором не найдено, бросает исключение.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Event GetEvent(Guid id);

        /// <summary>
        /// Асинхронно получает событие по его иденификатору. Если событие с таким идентификатором не найдено, бросает исключение.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        public Task<Event> GetEventAsync(Guid id, CancellationToken ct);

        /// <summary>
        /// Получает все события из хранилища и возвращает их в виде списка.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Event> GetEvents();

        
    }
}

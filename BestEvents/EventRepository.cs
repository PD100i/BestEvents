
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using BestEvents.Exceptions;


namespace BestEvents
{
    /// <summary>
    /// Хранилище для событий Event. Реализация интерфейса IEventRepository
    /// </summary>
    public class EventRepository : IEventRepository
    {

        static readonly ConcurrentDictionary<Guid, Event> events = new();

        /// <summary>
        /// Создает новый объект Event и добавляет в хранилище
        /// </summary>
        /// <param name="title">Название события</param>
        /// <param name="startAt">Дата начала</param>
        /// <param name="endAt">Дата завершения</param>
        /// <param name="description">Опсание (необязательное поле)</param>
        public void CreateEvent(string title, DateTime startAt, DateTime endAt, string description = "")
        {
            Guid id = Guid.NewGuid();
            events.TryAdd(id, new Event(id, title, startAt, endAt, description));
        }

        /// <summary>
        /// Удаляет событие из хранилища
        /// </summary>
        /// <param name="id"></param>
        public void RemoveEvent(Guid id)
        {
            events.TryRemove(id, out _);
        }

        /// <summary>
        /// Возвращает из хранилища событие Event по его идентификатору
        /// Если идентификатор не найден, возвращает null
        /// </summary>
        /// <param name="id">Идентификационный номер события</param>
        /// <returns></returns>
        public Event GetEvent(Guid id)
        {
            if (!events.ContainsKey(id))
                throw new EventsNotFoundException($"Событие с идентификатором {id} не найдено");
            return events[id];
        }

        /// <summary>
        /// Возвращает спиок всех событий в хранилище
        /// </summary>
        /// <returns></returns>
        [Produces("List<Event>")]
        public List<Event> GetAll()
        {
            List<Event> result = [..events.Values];
            if (result.Count > 0)
                return result;
            throw new EventsNotFoundException("Ни одного события не найдено");
        }

        /// <summary>
        /// Перезаписывает событие в хранилище,
        /// если событие с таким идентификатором существует
        /// </summary>
        /// <returns></returns>
        public void ReplaceEvent(Event _event)
        {
            if (events.ContainsKey(_event.Id))
                events[_event.Id] = _event;
        }        
    }
}

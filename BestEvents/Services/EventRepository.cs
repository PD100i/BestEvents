
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using BestEvents.Exceptions;
using System.ComponentModel;


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
        public Event CreateEvent(string title, DateTime? startAt, DateTime? endAt, string? description)
        {
            Guid id = Guid.NewGuid();
            Event newEvent = new(id, title, startAt, endAt, description);
            events.TryAdd(id, newEvent);
            return newEvent;
        }

        /// <summary>
        /// Удаляет событие из хранилища
        /// </summary>
        /// <param name="id"></param>
        public void RemoveEvent(Guid id)
        {
            if (!events.TryRemove(id, out _))
                throw new EventsNotFoundException($"Событие с идентификатором {id} не найдено. Удаление не произведено");
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
        /// Перезаписывает событие в хранилище,
        /// если событие с таким идентификатором существует
        /// </summary>
        /// <returns></returns>
        public void ReplaceEvent(Event _event)
        {
            if (!events.ContainsKey(_event.Id))
                throw new EventsNotFoundException($"Событие с идентификатором {_event.Id} не найдено. Обновление не произведено");
            events[_event.Id] = _event;
        }

        /// <summary>
        /// Возвращает спиок всех событий в хранилище
        /// </summary>
        /// <returns></returns>
        
        public PaginatedResult<Event> GetEvents(string? title, DateTime? from, DateTime? to, int page = 1, int size = 10)
        {
            var result = FilterEventsByTitle(events.Values, title);
            result = FilterEventsByDateFrom(result, from);
            result = FilterEventsByDateTo(result, to);

            int totalCount = result.Count();
            result = result.Skip(page - 1).Take(size);
            return new PaginatedResult<Event>(result.ToList(), page, totalCount);
        }

        private IEnumerable<Event> FilterEventsByTitle(IEnumerable<Event> events, string? title)
        {
            if (title == null)
                return events;
            return events.Where(e => IsContained(e.Title, title));
        }
        private IEnumerable<Event> FilterEventsByDateFrom(IEnumerable<Event> events, DateTime? from)
        {
            if (from == null)
                return events;
            return events.Where(e => e.StartAt >= from);
        }
        private IEnumerable<Event> FilterEventsByDateTo(IEnumerable<Event> events, DateTime? to)
        {
            if (to == null)
                return events;
            return events.Where(e => e.EndAt <= to);
        }
        private bool IsContained(string title, string value)
        {
            return title.Contains(value, StringComparison.OrdinalIgnoreCase);
        }

    }
}

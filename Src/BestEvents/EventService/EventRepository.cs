
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using BestEvents.Exceptions;
using System.ComponentModel;


namespace BestEvents
{
    /// <summary>
    /// Репозиторий для событий Event. Реализация интерфейса IEventRepository
    /// </summary>
    public class EventRepository() : IEventRepository
    {

        static readonly ConcurrentDictionary<Guid, Event> events = new();

        /// <inheritdoc/>
        public Event AddEvent(Event _event)
        {
            events.TryAdd(_event.Id, _event);
            return _event;
        }

        /// <inheritdoc/>
        public void RemoveEvent(Guid id)
        {
            if (!events.TryRemove(id, out _))
                throw new EventNotFoundException(string.Format(Messages_ru.EventNotDeleted, id));
        }

        /// <inheritdoc/>
        public Event GetEvent(Guid id)
        {
            if (!events.TryGetValue(id, out Event? value))
                throw new EventNotFoundException(string.Format(Messages_ru.EventNotFound, id));
            return value;
        }

        /// <inheritdoc/>
        public void ReplaceEvent(Event _event)
        {
            if (!events.ContainsKey(_event.Id))
                throw new EventNotFoundException(string.Format(Messages_ru.EventNotReplaced, _event.Id));
            events[_event.Id] = _event;
        }

        /// <inheritdoc/>
        public IEnumerable<Event> GetEvents()
        {
            return events.Values;
        }

        
    }
}


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
        public bool RemoveEvent(Guid id)
        {
            return events.TryRemove(id, out _);
                
        }

        /// <inheritdoc/>
        public Event? GetEvent(Guid id)
        {
            if (!events.TryGetValue(id, out Event? value))
                return null;
            return value;
        }

        /// <inheritdoc/>
        public bool ReplaceEvent(Event _event)
        {
            if (!events.ContainsKey(_event.Id))
                return false;
            events[_event.Id] = _event;
            return true;
        }

        /// <inheritdoc/>
        public IEnumerable<Event> GetEvents()
        {
            return events.Values;
        }

        
    }
}

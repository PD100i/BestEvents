
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
                throw new DataNotFoundException($"Событие с идентификатором {id} не найдено. Удаление не произведено");
        }

        /// <inheritdoc/>
        public Event GetEvent(Guid id)
        {
            if (!events.TryGetValue(id, out Event? value))
                throw new DataNotFoundException($"Событие с идентификатором {id} не найдено");
            return value;
        }


        /// <inheritdoc/>
        public async Task<Event> GetEventAsync(Guid id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            if (!events.TryGetValue(id, out Event? value))
                throw new DataNotFoundException($"Событие с идентификатором {id} не найдено");
            return await Task.FromResult(value);
        }

        /// <inheritdoc/>
        public void ReplaceEvent(Event _event)
        {
            if (!events.ContainsKey(_event.Id))
                throw new DataNotFoundException($"Событие с идентификатором {_event.Id} не найдено. Обновление не произведено");
            events[_event.Id] = _event;
        }

        /// <inheritdoc/>
        public IEnumerable<Event> GetEvents()
        {
            return events.Values;
        }

        
    }
}

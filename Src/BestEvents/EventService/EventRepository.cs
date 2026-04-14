
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
        public async Task<Event> AddEventAsync(Event _event)
        {
            events.TryAdd(_event.Id, _event);
            return await Task.FromResult(_event);
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveEventAsync(Guid id)
        {
            return await Task.FromResult(events.TryRemove(id, out _));
                
        }

        /// <inheritdoc/>
        public async Task<Event?> GetEventAsync(Guid id)
        {
            if (!events.TryGetValue(id, out Event? value))
                return null;
            return await Task.FromResult(value);
        }

        /// <inheritdoc/>
        public async Task<bool> ReplaceEventAsync(Event _event)
        {
            if (!events.ContainsKey(_event.Id))
                return await Task.FromResult(false);
            events[_event.Id] = _event;
            return await Task.FromResult(true);
        }

        /// <inheritdoc/>
        public async Task<IQueryable<Event>> GetEventsAsync()
        {
            return await Task.FromResult(events.Values.AsQueryable());
        }

        
    }
}

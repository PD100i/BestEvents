
using BestEvents.Exceptions;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;

namespace BestEvents
{
    public class EventRepository : IEventsRepository
    {
        static readonly ConcurrentDictionary<Guid, Event> events = new();

        public void CreateEvent(string title, DateTime startAt, DateTime endAt, string desctiption = "")
        {
            Guid id = Guid.NewGuid();
            events.TryAdd(id, new Event(id, title, startAt, endAt, desctiption));
        }

        public void RemoveEvent(Guid id)
        {
            events.TryRemove(id, out _);
        }

        public Event GetEvent(Guid id)
        {
            return events[id];
        }

        public List<Event> GetAll()
        {
            return [..events.Values];
        }

        public void ReplaceEvent(Event _event)
        {
            events[_event.Id] = _event;
        }        
    }
}

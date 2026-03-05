
using BestEvents.Exceptions;

namespace BestEvents
{
    public class EventRepository : IEventsRepository
    {
        static readonly List<Event> events = [];

        public void AddEvent(Event _event)
        {
            events.Add(_event);
        }

        public void RemoveEvent(Guid id)
        {
            Event? _event = FindEvent(id) ?? throw new NotFoundException();
            events.Remove(_event);
        }

        public Event GetEvent(Guid id)
        {
            return FindEvent(id) ?? throw new NotFoundException();
        }

        public List<Event> GetAll()
        {
            return events;
        }

        public void ReplaceEvent(Event _event)
        {
            int index = FindEventIndex(_event.Id);
            if (index == -1)
                throw new NotFoundException();
            events[index] = _event;
        }

        private Event? FindEvent(Guid id)
        {
            return events.Find(e => e.Equals(id));
        }

        private int FindEventIndex(Guid id)
        {
            return events.FindIndex(e => e.Equals(id));
        }

        
    }
}

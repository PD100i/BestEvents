
using BestEvents.Exceptions;

namespace BestEvents
{
    public class EventService : IEventService
    {
        readonly EventRepository repository;

        public EventService(EventRepository repository)
        {
            this.repository = repository;
        }

        
        public void CreateEvent(EventDto _event)
        {
            repository.CreateEvent(_event.Title, _event.StartAt, _event.endAt, _event.Description);
        }

        public void DeleteEvent(string id)
        {
            repository.RemoveEvent(GuidFromString(id));
        }

        public EventDtoExtended GetEvent(string id)
        {
            return GetDtoFromEvent(repository.GetEvent(GuidFromString(id)));
        }

        public List<EventDtoExtended> GetEvents()
        {
            List<EventDtoExtended> dto = [];
            repository.GetAll().ForEach(i => dto.Add(GetDtoFromEvent(i))) ;
            return dto;
        }

        public void ReplaceEvent(EventDtoExtended _event)
        {
            repository.ReplaceEvent(GetEventFromDto(_event));
        }

        private EventDtoExtended GetDtoFromEvent (Event _event)
        {
            return new EventDtoExtended(_event.Id.ToString(), _event.Title,  _event.StartAt, _event.EndAt, _event.Description);
        }

        private Event GetEventFromDto(EventDtoExtended _event)
        {
            return new Event(GuidFromString(_event.id), _event.Title, _event.StartAt, _event.EndAt, _event.Description);
        }
        private Guid GuidFromString(string id)
        {
            if (Guid.TryParse(id, out var _id))
                return _id;
            throw new ArgumentException("Неверный формат параметра id");
        }
    }
}

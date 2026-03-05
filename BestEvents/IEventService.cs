namespace BestEvents
{
    public interface IEventService
    {
        List<EventDtoExtended> GetEvents();

        EventDtoExtended GetEvent(string id);

        void CreateEvent(EventDto _event);

        void ReplaceEvent(EventDtoExtended _event);

        void DeleteEvent(string id);
    }
}

namespace BestEvents
{
    public interface IEventService
    {
        List<EventDtoExtended> GetEvents();

        EventDtoExtended GetEvent(string id);

        // void CreateEvent(string title, DateTime startAt, DateTime endAt, string description = "");

        void CreateEvent(EventDto eventDto);

        void ReplaceEvent(EventDtoExtended _event);

        void DeleteEvent(string id);
    }
}

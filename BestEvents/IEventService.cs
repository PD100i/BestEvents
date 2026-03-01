namespace BestEvents
{
    public interface IEventService
    {
        List<Event> GetEvents();

        Event GetEvent(int id);

        void AddEvent(Event @event);

        void ReplaceEvent(Event @event);

        void DeleteEvent(int id);
    }
}

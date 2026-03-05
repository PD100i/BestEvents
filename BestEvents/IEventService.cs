namespace BestEvents
{
    public interface IEventService
    {
        List<Event> GetEvents();

        Event GetEvent(int id);

        void CreateEvent(string title, string descriptor, DateTime startAt, DateTime endAt);

        void ReplaceEvent(Event @event);

        void DeleteEvent(int id);
    }
}

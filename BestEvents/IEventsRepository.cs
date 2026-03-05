namespace BestEvents
{
    public interface IEventsRepository
    {
        void CreateEvent(string title, string desctiption, DateTime startAt, DateTime endAt);

        void RemoveEvent(Guid id);

        void ReplaceEvent(Event _event);

        Event? GetEvent(Guid id);

        List<Event> GetAll();

        
    }
}

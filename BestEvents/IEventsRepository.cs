namespace BestEvents
{
    public interface IEventsRepository
    {
        void AddEvent(Event _event);

        void RemoveEvent(Guid id);

        void ReplaceEvent(Event _event);

        Event? GetEvent(Guid id);

        List<Event> GetAll();

        
    }
}

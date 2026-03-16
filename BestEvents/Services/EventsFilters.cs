using System.Collections;

namespace BestEvents
{
    /// <summary>
    /// Фильтр для событий Event. Содержит методы для фильтрации событий по названию, дате начала и дате завершения.
    /// </summary>
    public class EventsFilters
    {
        /// <summary>
        /// Фильтрует события по названию. Если название не указано, возвращает все события. Иначе возвращает только те события, 
        /// в названии которых содержится указанная строка (без учета регистра).
        /// </summary>
        /// <param name="events"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public IEnumerable<Event> FilterEventsByTitle(IEnumerable<Event> events, string? title)
        {
            if (title == null)
                return events;
            return events.Where(e => e.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Фильтрует события по дате начала. Если дата начала не указана, возвращает все события. Иначе возвращает только те события, 
        /// которые начались в указанную дату или позже.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public IEnumerable<Event> FilterEventsByDateFrom(IEnumerable<Event> events, DateTime? from)
        {
            if (from == null)
                return events;
            return events.Where(e => e.StartAt >= from);
        }

        /// <summary>
        /// Фильтрует события по дате завершения. Если дата завершения не указана, возвращает все события. Иначе возвращает только те события,
        /// которые завершились в указанную дату или раньше.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public IEnumerable<Event> FilterEventsByDateTo(IEnumerable<Event> events, DateTime? to)
        {
            if (to == null)
                return events;
            return events.Where(e => e.EndAt <= to);
        }
    }
}

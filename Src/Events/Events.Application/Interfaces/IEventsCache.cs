using Events.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application
{
    public interface IEventsCache
    {
        /// <summary>
        /// Возвращает событие по его идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Event> GetEventAsync(Guid id, CancellationToken ct = default);


        /// <summary>
        /// Возвращает список популярных событий
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<List<Event>> GetTopPopularEventsAsync(CancellationToken ct = default);

        /// <summary>
        /// Удаляет
        /// </summary>
        /// param name="id"></param>
        /// <returns></returns>
        Task InvalidateEventAsync(Guid id, CancellationToken ct = default);

    }
}

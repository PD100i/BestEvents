using BestEvents.Exceptions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BestEvents
{
    /// <summary>
    /// Сервис событий, реализующий интерфейс IEventService. 
    /// </summary>
    public class EventService(AppDbContext db, EntityMapper mapper, EventFilters filters, Pagination<EventEntity> pagination) : IEventService
    {
        /// <inheritdoc/>
        public async Task<Event> CreateEventAsync(Event _event, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var entity = mapper.MapEventToEntity(_event);
            await db.Events.AddAsync(entity, ct);
            await db.SaveChangesAsync(ct);
            return _event;
        }

        /// <inheritdoc/>
        public async Task DeleteEventAsync(Guid id, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var eventEntity = await db.Events.FindAsync(id, ct);
            if (eventEntity == null)
                throw new EventNotFoundException(string.Format(Messages_ru.EventNotDeleted, id));
            db.Events.Remove(eventEntity);
            await db.SaveChangesAsync(ct);

        }

        /// <inheritdoc/>
        public async Task<Event> GetEventAsync(Guid id, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var eventEntity = await db.Events.FirstOrDefaultAsync(e => e.Id == id, ct);
            if (eventEntity == null)
                throw new EventNotFoundException(string.Format(Messages_ru.EventNotFound, id));
            return mapper.MapEntityToEvent(eventEntity);

        }

        /// <inheritdoc/>
        public async Task<PaginatedResult<Event>> GetEventsAsync(string? title, DateTime? from, DateTime? to, int page = 1, int size = 10, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            if (from != null && to != null && from > to)
                throw new EventWrongParameterException(Messages_ru.EndAt_Less_StartAt);
            if (page <= 0)
                throw new EventWrongParameterException(string.Format(Messages_ru.WrongPageForPagination, page));
            if (size <= 0)
                throw new EventWrongParameterException(string.Format(Messages_ru.WrongSizeForPagination, size));
                    
            var filtredResult = filters.FilterEventsByTitle(db.Events, title);
            filtredResult = filters.FilterEventsByDateFrom(filtredResult, from);
            filtredResult = filters.FilterEventsByDateTo(filtredResult, to);
            var result = pagination.GetResult(filtredResult, page, size);

            return await Task.FromResult(mapper.MapPaginatedResultToEntity(result));
        }

        /// <inheritdoc/>
        public async Task ReplaceEventAsync(Guid id, Event _event, CancellationToken ct = default)
        {
            if (id != _event.Id)
                throw new EventWrongParameterException(string.Format(Messages_ru.MismatchIdInReplaceRequest, id, _event.Id));
            ct.ThrowIfCancellationRequested();
            var eventEntity = await db.Events.FindAsync(_event.Id, ct);
            if (eventEntity == null)
                throw new EventNotFoundException(string.Format(Messages_ru.EventNotReplaced, _event.Id));
            mapper.UpdateEventEntity(_event, eventEntity);
            db.Events.Update(eventEntity);
            await db.SaveChangesAsync(ct);           
        }

        
    }
}

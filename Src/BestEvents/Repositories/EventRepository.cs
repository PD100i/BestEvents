using BestEvents.Exceptions;
using BestEvents.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace BestEvents
{
    /// <summary>
    /// Сервис событий, реализующий интерфейс IEventService. 
    /// </summary>
    public class EventRepository(AppDbContext db, EntityMapper mapper, EventFilters filters, Pagination<EventEntity> pagination) : IEventRepository
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

        
        /// <summary>
        /// Обновляет существующее событие
        /// </summary>
        /// <param name="_event"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Event> UpdateEventAsync(Event _event, CancellationToken ct = default)
        {
            var eventEntity = await db.Events.FirstOrDefaultAsync(e => e.Id == _event.Id, ct);
            if (eventEntity == null)
                throw new EventNotFoundException(string.Format(Messages_ru.EventNotFound, _event.Id));
            mapper.UpdateEventEntity(_event, eventEntity);
            db.Events.Update(eventEntity);

            await db.SaveChangesAsync(ct);
            if (db.Database.CurrentTransaction != null)
                await db.Database.CurrentTransaction.CommitAsync(ct);
            return _event;
        }


        /// <inheritdoc/>
        public async Task<Event> UpdateEventAsync(Guid id, Func<Event, Task> action, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await using var transaction = db.Database.CurrentTransaction ?? await db.Database.BeginTransactionAsync(ct);

            try
            {
                var eventEntity = await db.Events.FromSql(
                $"SELECT * FROM events WHERE id = {id} FOR UPDATE")
                .FirstOrDefaultAsync(ct);

                if (eventEntity == null)
                    throw new EventNotFoundException(string.Format(Messages_ru.CreateBookingEventNotFound, id));

                Event _event = mapper.MapEntityToEvent(eventEntity);

                await action(_event);

                mapper.UpdateEventEntity(_event, eventEntity);
                db.Events.Update(eventEntity);

                await db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return _event;
            }
            finally
            {
                await db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
        }
    }
}

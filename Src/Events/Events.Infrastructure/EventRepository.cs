using Microsoft.EntityFrameworkCore;
using Events.Domain;
using Events.Application;
using Events.Application.Exceptions;
using Common;


namespace Events.Infrastructure
{
    /// <summary>
    /// Сервис событий, реализующий интерфейс IEventService. 
    /// </summary>
    public class EventRepository(AppDbContext db, EntityMapper mapper, EventFilters filters, Pagination<EventEntity> pagination) : IEventRepository
    {
        /// <inheritdoc/>
        public async Task<Event> AddEventAsync(Event _event, CancellationToken ct = default)
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
        public async Task<Event> GetEventForUpdateAsync(Guid id, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            int operationTimeout = 2000;
            await db.Database.ExecuteSqlRawAsync("SELECT set_config('lock_timeout', {0}, true);", operationTimeout.ToString());

            var eventEntity = await db.Events.FromSql(
                $"SELECT * FROM events WHERE id = {id} FOR UPDATE")
                .FirstOrDefaultAsync(ct);

            if (eventEntity == null)
                throw new EventNotFoundException(string.Format(Messages_ru.EventNotFound, id));

            return mapper.MapEntityToEvent(eventEntity);
        }

        /// <inheritdoc/>
        public async Task<PaginatedResult<Event>> GetEventsAsync(string? title, DateTime? from, DateTime? to, int page = 1, int size = 10, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var events = db.Events;

            var filtredResult = filters.FilterEventsByTitle(db.Events, title);
            filtredResult = filters.FilterEventsByDateFrom(filtredResult, from);
            filtredResult = filters.FilterEventsByDateTo(filtredResult, to);
            var orderedResult = filtredResult.OrderBy(e => e.StartAt);
            var result = pagination.GetResult(orderedResult, page, size);
            
            return await Task.FromResult(mapper.MapPaginatedResultToEntity(result));
        }

        
        /// <inheritdoc/>
        public async Task<Event> ReplaceEventAsync(Event _event, CancellationToken ct = default)
        {
            try
            {
                var existingEvent = await db.Events.FirstOrDefaultAsync(e => e.Id == _event.Id, ct);
                if (existingEvent == null)
                    throw new EventNotFoundException(Messages_ru.EventNotFound);
                mapper.UpdateEventEntity(_event, existingEvent);
                return _event;
            }
            catch (Exception ex)
            {
                throw new UpdateEventException(string.Format(Messages_ru.UpdateEventErrorMessage, _event.Id), ex);
            }
        }

        /// <inheritdoc/>
        public async Task AddToCancelledBookingInboxAsync(Message message, CancellationToken ct = default)
        {
            await db.BookingCancelledInbox.AddAsync(mapper.MapBookingMessageToEntity(message), ct);
        }
        /// <inheritdoc/>
        public async Task AddToCreatedBookingInboxAsync(Message message, CancellationToken ct = default)
        {
            await db.BookingCreatedInbox.AddAsync(mapper.MapBookingMessageToEntity(message), ct);
        }

        /// <inheritdoc/>
        public async Task<Message?> GetUnpublishedSeatsReservationErrorAsync(CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var message = await db.SeatsReservationErrorOutbox
                .OrderBy(m => m.CreatedAt)
                .FirstOrDefaultAsync(ct);
            if (message == null)
                return null;
            return mapper.MapBookingMessageEntityToMessage(message);
        }

        /// <inheritdoc/>
        public async Task<Message?> GetUnpublishedSeatsReservedAsync(CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var message = await db.SeatsReservedOutbox
                .OrderBy(m => m.CreatedAt)
                .FirstOrDefaultAsync(ct);
            if (message == null)
                return null;
            return mapper.MapBookingMessageEntityToMessage(message);
        }

        /// <inheritdoc/>
        public async Task DequeueSeatsReservationErrorAsync(Guid bookingId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await db.SeatsReservationErrorOutbox
                .Where(m => m.BookingId == bookingId)
                .ExecuteDeleteAsync(ct);
        }

        /// <inheritdoc/>
        public async Task DequeueSeatsReservedAsync(Guid bookingId, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await db.SeatsReservedOutbox
                .Where(m => m.BookingId == bookingId)
                .ExecuteDeleteAsync(ct);
        }

        /// <inheritdoc/>
        public async Task EnqueueSeatsReservationErrorAsync(Message message, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await db.SeatsReservationErrorOutbox.AddAsync(mapper.MapBookingMessageToEntity(message), ct);
        }

        /// <inheritdoc/>
        public async Task EnqueueSeatsReservedAsync(Message message, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            await db.SeatsReservedOutbox.AddAsync(mapper.MapBookingMessageToEntity(message), ct);
        }

    }
}

using Common;
using Confluent.Kafka;
using Events.Application.Exceptions;
using Events.Domain;
using Events.Domain.Exceptions;
using Microsoft.Extensions.Logging;


namespace Events.Application
{
    /// <summary>
    /// Сервис событий, реализующий интерфейс IEventService. 
    /// </summary>
    public class EventService(IEventRepository repository, IUnitOfWork uow, ILogger<EventService> logger) : IEventService
    {
        /// <inheritdoc/>
        public async Task<Event> CreateEventAsync(Event _event, CancellationToken ct = default)
        {
            await repository.AddEventAsync(_event, ct);
            await uow.SaveChangesAsync(ct);
            return _event;
        }

        /// <inheritdoc/>
        public async Task DeleteEventAsync(Guid id, CancellationToken ct = default)
        {
            await repository.DeleteEventAsync(id, ct);
            await uow.SaveChangesAsync(ct);
        }

        /// <inheritdoc/>
        public async Task<Event> GetEventAsync(Guid id, CancellationToken ct = default)
        {
            return await repository.GetEventAsync(id, ct);
        }

        /// <inheritdoc/>
        public async Task<PaginatedResult<Event>> GetEventsAsync(string? title, DateTime? from, DateTime? to, int page = 1, int size = 10, CancellationToken ct = default)
        {
            return await repository.GetEventsAsync(title, from, to, page, size, ct);
        }

        /// <inheritdoc/>
        public async Task ReplaceEventAsync(Guid id, Event _event, CancellationToken ct = default)
        {
            if (id != _event.Id)
                throw new EventWrongParameterException(string.Format(Messages_ru.MismatchIdInReplaceRequest, id, _event.Id));  
            await repository.ReplaceEventAsync(_event, ct);
            await uow.SaveChangesAsync(ct);
        }

        /// <inheritdoc/>
        public async Task TryReleseSeats(Guid eventId, Guid bookingId, CancellationToken ct = default)
        {
            Event? _event = null;
            

            try
            {
                _event = await repository.GetEventAsync(eventId, ct);
                _event.TryReserveSeats();
                await repository.ReplaceEventAsync(_event, ct);
                await uow.SaveChangesAsync(ct);
                logger.LogInformation(string.Format(Messages_ru.SeatsReleased, eventId, bookingId));

            }
            catch (Exception ex)
            {
                logger.LogWarning(string.Format(Messages_ru.ErrorReserveSeats, eventId, bookingId) + " " + ex.Message);
            }
        }

        /// <inheritdoc/>
        public async Task TryReserveSeats(Guid eventId, Guid bookingId, CancellationToken ct = default)
        {
            var message = new BookingMessage
            {
                BookingId = bookingId,
                EventId = eventId,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var _event = await repository.GetEventAsync(eventId, ct);
                _event.TryReserveSeats();
                await repository.EnqueueSeatsReservedAsync(message, ct);
                await repository.ReplaceEventAsync(_event, ct);
                await uow.SaveChangesAsync(ct);
                logger.LogInformation(string.Format(Messages_ru.SeatsReserved, eventId, bookingId));

            }
            catch (Exception ex)
            {              
                await repository.EnqueueSeatsReservationErrorAsync(message, ct);
                logger.LogWarning(string.Format(Messages_ru.ErrorReserveSeats, eventId, bookingId) + " " + ex.Message);
            }
            
        }
    }
}

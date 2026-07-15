using Common;
using Confluent.Kafka;
using Events.Application.Exceptions;
using Events.Domain;
using Events.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;



namespace Events.Application
{
    /// <summary>
    /// Сервис событий, реализующий интерфейс IEventService. 
    /// </summary>
    public class EventService(IEventRepository repository, IUnitOfWork uow, IEventsCache cache, ILogger<EventService> logger) : IEventService
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
            await cache.InvalidateEventAsync(id, ct);
            await uow.SaveChangesAsync(ct);
        }

        /// <inheritdoc/>
        public async Task<Event> GetEventAsync(Guid id, CancellationToken ct = default)
        {
            return await cache.GetEventAsync(id, ct);
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
            await cache.InvalidateEventAsync(id, ct);
            await uow.SaveChangesAsync(ct);
        }

        /// <inheritdoc/>
        public async Task TryReleseSeats(Message message, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                using var transaction = await uow.BeginTransactionAsync();
                await repository.AddMessageToInboxAsync(message, ct);
                var _event = await repository.GetEventForUpdateAsync(message.EventId, ct);
                _event.ReleaseSeats();
                await repository.ReplaceEventAsync(_event, ct);
                await cache.InvalidateEventAsync(message.EventId, ct);
                await uow.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                logger.LogInformation(string.Format(Messages_ru.SeatsReleased, message.EventId, message.BookingId));

            }
            catch (Exception ex)
            {
                logger.LogWarning(string.Format(Messages_ru.ErrorReleaseSeats, message.EventId, message.BookingId) + " " + ex.Message);
            }
        }

       

        /// <inheritdoc/>
        public async Task TryReserveSeats(Message message, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                using var transaction = await uow.BeginTransactionAsync();
                await repository.AddMessageToInboxAsync(message, ct);
                Event _event = await repository.GetEventForUpdateAsync(message.EventId, ct);
                _event.TryReserveSeats();
                await repository.EnqueueMessageAsync(new Message() 
                {
                    Id = Guid.NewGuid(),
                    BookingId = message.BookingId,
                    EventId = message.EventId,
                    CreatedAt = DateTime.UtcNow,
                    MessageType = MessageTypeEnum.SeatsReserved
                }, ct);
                await repository.ReplaceEventAsync(_event, ct);
                await cache.InvalidateEventAsync(message.EventId, ct);
                await uow.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                logger.LogInformation(string.Format(Messages_ru.SeatsReserved, message.EventId, message.BookingId));

            }
            catch (EventNotFoundException ex)
            {
                uow.CleanContext();
                await repository.EnqueueMessageAsync(new Message()
                {
                    Id = Guid.NewGuid(),
                    BookingId = message.BookingId,
                    EventId = message.EventId,
                    CreatedAt = DateTime.UtcNow,
                    MessageType = MessageTypeEnum.ReservationSeatsError
                }, ct);
                await uow.SaveChangesAsync(ct);
                logger.LogInformation(string.Format(Messages_ru.ErrorReserveSeats, message.EventId, message.BookingId) + " " + ex.Message);
            }
            catch (ReserveSeatsException ex)
            {
                uow.CleanContext();
                await repository.EnqueueMessageAsync(new Message()
                {
                    Id = Guid.NewGuid(),
                    BookingId = message.BookingId,
                    EventId = message.EventId,
                    CreatedAt = DateTime.UtcNow,
                    MessageType = MessageTypeEnum.ReservationSeatsError
                }, ct);
                await uow.SaveChangesAsync(ct);
                logger.LogInformation(string.Format(Messages_ru.ErrorReserveSeats, message.EventId, message.BookingId) + " " + ex.Message);
            }
            catch(Exception ex)
            {
                logger.LogInformation(string.Format(Messages_ru.ErrorReserveSeats, message.EventId, message.BookingId) + " " + ex.Message);
            }
        }
    }
}

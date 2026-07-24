using Common;
using Events.Application.Exceptions;
using Events.Domain;
using Events.Domain.Exceptions;
using Microsoft.Extensions.Logging;




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
            try
            {    await cache.InvalidateEventAsync(_event.Id, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Ошибка при инвалидации кэша для события {EventId}.", _event.Id);
            }
            await uow.SaveChangesAsync(ct);
            return _event;
        }

        /// <inheritdoc/>
        public async Task DeleteEventAsync(Guid id, CancellationToken ct = default)
        {
            await repository.DeleteEventAsync(id, ct);
            try
            {
                await cache.InvalidateEventAsync(id, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Ошибка при инвалидации кэша для события {EventId}.", id);
            }
            await uow.SaveChangesAsync(ct);
        }

        /// <inheritdoc/>
        public async Task<Event> GetEventAsync(Guid id, CancellationToken ct = default)
        {
            Event? _event = null;
            try
            {
                _event = await cache.GetEventAsync(id, ct);

                if(_event == null)
                {
                    logger.LogWarning("Ошибка при чтении из кэша события по идентификатору {EventId}.", id);
                    _event = await repository.GetEventAsync(id, ct);
                }
            }
            catch(OperationCanceledException)
            {
                throw;
            }
           
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Ошибка при чтении из кэша события по идентификатору {EventId}.", id);
                _event = await repository.GetEventAsync(id, ct);
            }
            return _event ?? throw new EventNotFoundException(string.Format(Messages_ru.EventNotFound, id));
        }

        /// <inheritdoc/>
        public async Task<PaginatedResult<Event>> GetEventsAsync(string? title, DateTime? from, DateTime? to, int page = 1, int size = 10, CancellationToken ct = default)
        {
            return await repository.GetEventsAsync(title, from, to, page, size, ct);
        }

        /// <inheritdoc/>
        public async Task<List<Event>> GetTopPopularEventsAsync(CancellationToken ct = default)
        {
            try
            {                 
                return await cache.GetTopPopularEventsAsync(ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Ошибка при чтении из кэша топ-10 событий по популярности.");
                return await repository.GetTopPopularEventsAsync(ct);
            }
        }

        /// <inheritdoc/>
        public async Task ReplaceEventAsync(Guid id, Event _event, CancellationToken ct = default)
        {
            if (id != _event.Id)
                throw new EventWrongParameterException(string.Format(Messages_ru.MismatchIdInReplaceRequest, id, _event.Id));  
            await repository.ReplaceEventAsync(_event, ct);
            try
            {
                await cache.InvalidateEventAsync(id, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Ошибка инвалидации кэша для события {EventId}", id);
            }
            await uow.SaveChangesAsync(ct);
        }

        /// <inheritdoc/>
        public async Task TryReleaseSeats(Message message, CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                using var transaction = await uow.BeginTransactionAsync();
                await repository.AddMessageToInboxAsync(message, ct);
                var _event = await repository.GetEventForUpdateAsync(message.EventId, ct)
                    ?? throw new EventNotFoundException(string.Format(Messages_ru.EventNotFound, message.EventId));
                _event.ReleaseSeats();
                await repository.ReplaceEventAsync(_event, ct);
                try
                {
                    await cache.InvalidateEventAsync(message.EventId, ct);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Ошибка инвалидации кэша для события {EventId}", message.EventId);
                }
                await uow.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                logger.LogInformation("Освобождено место на событие {EventId}, зарезервированное для бронирования {BookingId}", message.EventId, message.BookingId);

            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Не удалось освободить место на событие {EventId}, зарезервированное для бронирования {BookingId}.", message.EventId, message.BookingId);
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
                Event _event = await repository.GetEventForUpdateAsync(message.EventId, ct) 
                    ?? throw new EventNotFoundException(string.Format(Messages_ru.EventNotFound, message.EventId));
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
                try
                {
                    await cache.InvalidateEventAsync(message.EventId, ct);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Ошибка инвалидации кэша для события {EventId}", message.EventId);
                }
                await uow.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                logger.LogInformation("Зарезервировано место на событие {EventId} для бронирования {BookingId}", message.EventId, message.BookingId);

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
                logger.LogInformation(ex, "Не удалось зарезервировать место на событие {EventId} для бронирования {BookingId}.", message.EventId, message.BookingId);
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
                logger.LogInformation(ex, "Не удалось зарезервировать место на событие {EventId} для бронирования {BookingId}.", message.EventId, message.BookingId);
            }
            catch(Exception ex)
            {
                logger.LogInformation(ex, "Не удалось зарезервировать место на событие {EventId} для бронирования {BookingId}.", message.EventId, message.BookingId);
            }
        }
    }
}

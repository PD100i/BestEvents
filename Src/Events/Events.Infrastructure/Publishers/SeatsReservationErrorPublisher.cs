using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Events.Application;


namespace Events.Infrastructure
{
    public class SeatsReservationErrorPublisher(IServiceScopeFactory scopeFactory, SeatsReservationErrorProducer producer, ILogger<SeatsReservationErrorPublisher> logger)
        : BasePublisher(logger)
    {
        protected override async Task Publiсation(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IEventRepository>();


            var message = await repo.GetOldestUnpublishedMessageAsync(MessageTypeEnum.ReservationSeatsError, stoppingToken);
            if (message == null)
                return;
            logger.LogInformation("Найдена запись об ошибке резервирования мест: EventId - {EventId}, BookingId - {BookingId}", message.EventId, message.BookingId);
            await producer.PublicationAsync(message.EventId.ToString(), message);
            await repo.DequeueMessageAsync(message.Id, stoppingToken);
            logger.LogInformation("Обработана запись об ошибке резервирования мест: EventId - {EventId}, BookingId - {BookingId}", message.EventId, message.BookingId);
        }
    }
}

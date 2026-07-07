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


            var message = await repo.GetUnpublishedSeatsReservationErrorAsync(stoppingToken);
            if (message == null)
                return;

            await producer.PublicationAsync(message.BookingId.ToString(), message);
            await repo.DequeueSeatsReservationErrorAsync(message.BookingId, stoppingToken);
        }
    }
}

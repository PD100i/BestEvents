using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Events.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure
{
    internal class SeatsReservedPublisher(IServiceScopeFactory scopeFactory, SeatsReservedProducer producer, ILogger<SeatsReservedPublisher> logger)
        : BasePublisher(logger)
    {
        protected override async Task Publiсation(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IEventRepository>();


            var message = await repo.GetOldestUnpublishedMessageAsync(MessageTypeEnum.SeatsReserved, stoppingToken);
            if (message == null)
                return;
            logger.LogInformation("Найдена запись о резервировании мест: EventId - {EventId}, BookingId - {BookingId}", message.EventId, message.BookingId);
            await producer.PublicationAsync(message.EventId.ToString(), message);
            await repo.DequeueMessageAsync(message.Id, stoppingToken);
            logger.LogInformation("Обработана запись о резервировании мест: EventId - {EventId}, BookingId - {BookingId}", message.EventId, message.BookingId);
        }
    }
}

using Bookings.Application.Exceptions;
using Bookings.Application;
using Common;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bookings.Infrastructure
{
    /// <summary>
    /// Фоновый сервис для обработки бронирований
    /// </summary>
    public class BookingCreatedPublisher(IServiceScopeFactory scopeFactory, BookingCreatedProducer producer, ILogger<BookingCreatedPublisher> logger) 
        : BasePublisher(logger)
    {
        protected override async Task Publiсation(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();


            var message = await repo.GetOldestUnpublishedMessageAsync(MessageTypeEnum.BookingCreated, stoppingToken);
            if (message == null)
                return;
            logger.LogInformation("Найдена запись о создании бронирования: EventId - {EventId}, BookingId - {BookingId}", message.EventId, message.BookingId);
            await producer.PublicationAsync(message.EventId.ToString(), message);
            await repo.DequeueMessageAsync(message.Id, stoppingToken);
        }
    }
}

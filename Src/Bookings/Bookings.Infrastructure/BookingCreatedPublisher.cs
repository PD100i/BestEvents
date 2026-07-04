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
    public class BookingCreatedPublisher(IServiceScopeFactory scopeFactory, BaseProducer<string, BookingCreatedMessage> producer, ILogger<BookingCreatedPublisher> logger) 
        : AbstractPublisher(logger)
    {
        const int MaxPublishedMessagesQuantity = 10;

        protected override async Task Publiсation(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();


            var messages = await repo.GetUnpublishedCreatedBookingsAsync(MaxPublishedMessagesQuantity, stoppingToken);
            if (messages == null || messages.Count == 0)
                return;

            await producer.PublishAsync();
            await repo.DequeueBookingCreatedAsync(messages.Select(m => m.BookingId), stoppingToken);
        }
    }
}

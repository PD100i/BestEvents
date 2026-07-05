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
    public class BookingCancelledPublisher(IServiceScopeFactory scopeFactory, BaseProducer<string, BookingCreatedMessage> producer, ILogger<BookingCreatedPublisher> logger) 
        : AbstractPublisher(logger)
    {
        protected override async Task Publiсation(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();


            var message = await repo.GetUnpublishedCreatedBookingAsync(stoppingToken);
            if (message == null)
                return;

            await producer.PublicationAsync(message.BookingId.ToString(), message);
            await repo.DequeueBookingCreatedAsync(message.BookingId, stoppingToken);
        }
    }
}

using Common;
using Events.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure.Consumers
{
    public class BookingCancelledConsumer(IServiceScopeFactory scopeFactory, ILogger<BookingCancelledConsumer> logger)
        : BaseConsumer<BookingMessage>(Topics.BookingCreatedTopic, "booking_cancelled_group", logger)
    {
        protected override async Task ProcessMessageAsync(string key, BookingMessage value, CancellationToken ct)
        {
            using var scope = scopeFactory.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<EventRepository>();
            var service = scope.ServiceProvider.GetRequiredService<EventService>();

            await repository.AddToCratedBookingInboxAsync(value, ct);
            await service.TryReleseSeats(value.BookingId, value.EventId, ct);
        }
    }
}

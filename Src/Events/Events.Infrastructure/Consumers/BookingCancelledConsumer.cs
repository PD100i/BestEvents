using Common;
using Events.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure.Consumers
{
    public class BookingCancelledConsumer(IServiceScopeFactory scopeFactory, IOptions<KafkaSettings> options, ILogger<BookingCancelledConsumer> logger)
        : BaseConsumer<Message>(options.Value, Topics.BookingCreatedTopic, "booking_cancelled_group", logger)
    {
        protected override async Task ProcessMessageAsync(string key, Message value, CancellationToken ct)
        {
            using var scope = scopeFactory.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IEventService>();

            await service.TryReleseSeats(value, ct);
        }
    }
}

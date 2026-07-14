using Common;
using Events.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Events.Infrastructure
{
    public class BookingCreatedConsumer(IServiceScopeFactory scopeFactory, IOptions<KafkaSettings> options, ILogger<BookingCreatedConsumer> logger)
        : BaseConsumer<Message>(options.Value, Topics.BookingCreatedTopic, "booking_created_group", logger)
    {
        protected override async Task ProcessMessageAsync(string key, Message value, CancellationToken ct)
        {
            using var scope = scopeFactory.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IEventService>();
            await service.TryReserveSeats(value, ct);
        }
    }
}

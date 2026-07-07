using Common;
using Microsoft.Extensions.Logging;
using Events.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Events.Infrastructure
{
    public class BookingCreatedConsumer(IServiceScopeFactory scopeFactory, ILogger<BookingCreatedConsumer> logger)
        : BaseConsumer<BookingMessage>(Topics.BookingCreatedTopic, "booking_created_group", logger)
    {
        protected override async Task ProcessMessageAsync(string key, BookingMessage value, CancellationToken ct)
        {
            using var scope = scopeFactory.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<EventRepository>();
            var service = scope.ServiceProvider.GetRequiredService<EventService>();
            await repository.AddToCratedBookingInboxAsync(value, ct);
            await service.TryReserveSeats(value.BookingId, value.EventId, ct);
        }
    }
}

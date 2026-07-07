using Common;
using Microsoft.Extensions.Logging;
using Events.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Events.Infrastructure
{
    public class BookingCreatedConsumer(IEventService service, IEventRepository repository, ILogger<BookingCreatedConsumer> logger)
        : BaseConsumer<BookingMessage>(Topics.BookingCreatedTopic, "booking_created_group", logger)
    {
        protected override async Task ProcessMessageAsync(string key, BookingMessage value, CancellationToken ct)
        {
            await repository.AddToCratedBookingInboxAsync(value, ct);
            await service.TryReserveSeats(value.BookingId, value.EventId, ct);
        }
    }
}

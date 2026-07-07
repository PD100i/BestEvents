using Common;
using Events.Application;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure.Consumers
{
    public class BookingCancelledConsumer(IEventService service, IEventRepository repository, ILogger<BookingCancelledConsumer> logger)
        : BaseConsumer<BookingMessage>(Topics.BookingCreatedTopic, "booking_cancelled_group", logger)
    {
        protected override async Task ProcessMessageAsync(string key, BookingMessage value, CancellationToken ct)
        {
            await repository.AddToCratedBookingInboxAsync(value, ct);
            await service.TryReleseSeats(value.BookingId, value.EventId, ct);
        }
    }
}

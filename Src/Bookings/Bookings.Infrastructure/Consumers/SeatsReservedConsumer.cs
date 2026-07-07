using Bookings.Application;
using Microsoft.Extensions.Logging;
using Common;
using Microsoft.Extensions.DependencyInjection;

namespace Bookings.Infrastructure
{
    public class SeatsReservedConsumer(IServiceScopeFactory scopeFactory, ILogger<SeatsReservedConsumer> logger) 
        : BaseConsumer<BookingMessage>(Topics.SeatsReservedTopic, "seats_reserved_group", logger)
    {
        protected override async Task ProcessMessageAsync(string key, BookingMessage value, CancellationToken ct)
        {
            using var scope = scopeFactory.CreateScope() ;
            var service = scope.ServiceProvider.GetRequiredService<IBookingService>();

            await service.ConfirmBooking(value.BookingId, ct);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

    }
}

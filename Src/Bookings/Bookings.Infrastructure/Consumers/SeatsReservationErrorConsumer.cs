using Bookings.Application;
using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bookings.Infrastructure
{
    public class SeatsReservationErrorConsumer(IServiceScopeFactory scopeFactory, IOptions<KafkaSettings> options, ILogger<SeatsReservationErrorConsumer> logger) 
        : BaseConsumer<Message>(options.Value, Topics.SeatsReservationErrorTopic, "seats_reservation_error_group", logger)
    {
        protected override async Task ProcessMessageAsync(string key, Message value, CancellationToken ct)
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBookingService>();
            await service.RejectedBooking(value.BookingId, ct);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}

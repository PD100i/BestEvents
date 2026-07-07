using Bookings.Application;
using Microsoft.Extensions.Logging;
using Common;
using Microsoft.Extensions.DependencyInjection;

namespace Bookings.Infrastructure
{
    public class SeatsReservationErrorConsumer(IServiceScopeFactory scopeFactory, ILogger<SeatsReservationErrorConsumer> logger) 
        : BaseConsumer<BookingMessage>(Topics.SeatsReservationErrorTopic, "seats_reservation_error_group", logger)
    {
        protected override async Task ProcessMessageAsync(string key, BookingMessage value, CancellationToken ct)
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

using Bookings.Application;
using Microsoft.Extensions.Logging;
using Common;

namespace Bookings.Infrastructure
{
    public class BookingRejectedConsumer(IBookingService service, ILogger<BookingConfirmedConsumer> logger) 
        : BaseConsumer<string, BookingConfirmedMessage>("booking_rejected", "booking_rejected_group", logger)
    {
        protected override async Task ProcessMessageAsync(string key, BookingConfirmedMessage value, CancellationToken ct)
        {
            await service.RejectedBooking(value.BookingId, ct);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

    }
}

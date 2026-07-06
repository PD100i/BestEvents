using Bookings.Application;
using Microsoft.Extensions.Logging;
using Common;

namespace Bookings.Infrastructure
{
    public class BookingConfirmedConsumer(IBookingService service, ILogger<BookingConfirmedConsumer> logger) 
        : BaseConsumer<string, BookingConfirmedMessage>("booking_confirmed", "booking_confirmed_group", logger)
    {
        protected override async Task ProcessMessageAsync(string key, BookingConfirmedMessage value, CancellationToken ct)
        {
            await service.ConfirmBooking(value.BookingId, ct);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

    }
}

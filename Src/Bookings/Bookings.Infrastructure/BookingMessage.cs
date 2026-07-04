

namespace Bookings.Infrastructure
{
    public class BookingMessage
    {
        public Guid BookingId { get; set; }

        public Guid EventId { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}

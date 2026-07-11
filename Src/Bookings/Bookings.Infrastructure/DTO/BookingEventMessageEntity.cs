

namespace Bookings.Infrastructure
{
    public class BookingEventMessageEntity
    {
        public required Guid Id { get; set; }

        public Guid BookingId { get; set; }

        public Guid EventId { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}

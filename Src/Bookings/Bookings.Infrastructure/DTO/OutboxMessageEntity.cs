

using Common;

namespace Bookings.Infrastructure
{
    public class OutboxMessageEntity
    {
        public required Guid Id { get; set; }

        public Guid BookingId { get; set; }

        public Guid EventId { get; set; }

        public DateTime CreatedAt { get; set; }

        public MessageTypeEnum MessageType { get; set; }

    }
}

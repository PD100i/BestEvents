using System.Text.Json;

namespace Common
{
    public class Message
    {
        public required Guid Id { get; set; }

        public Guid BookingId { get; set; }

        public Guid EventId { get; set; }

        public required DateTime CreatedAt { get; set; }

        public MessageTypeEnum MessageType { get; set; }

    }

    public enum MessageTypeEnum
    {
        BookingCreated,
        BookingCancelled,
        SeatsReserved,
        ReservationSeatsError
    }
}

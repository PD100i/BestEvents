namespace Common
{
    public record CreatedBookingMessage
    {
        public Guid BookingId { get; set; }

        public Guid EventId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

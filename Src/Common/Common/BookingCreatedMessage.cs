namespace Common
{
    public record BookingCreatedMessage
    {
        public Guid BookingId { get; set; }

        public Guid EventId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

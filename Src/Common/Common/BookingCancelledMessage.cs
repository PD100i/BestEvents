

namespace Common
{
    public class BookingCancelledMessage
    {
        public Guid Key { get; set; }

        public Guid BookingId { get; set; }

        public DateTime CancelledAt { get; set; }

    }
}



namespace Common
{
    public class BookingCancelledMessage
    {
        public Guid BookingId { get; set; }

        public DateTime CancelledAt { get; set; }

    }
}

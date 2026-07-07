using System.Text.Json;

namespace Common
{
    public class BookingMessage
    {
        public Guid BookingId { get; set; }

        public Guid EventId { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}

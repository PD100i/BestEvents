using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure
{
    public class BookingEventMessageEntity
    {
        public required Guid Id { get; set; }

        public Guid BookingId { get; set; }

        public Guid EventId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

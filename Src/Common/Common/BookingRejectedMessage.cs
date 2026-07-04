using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class BookingRejectedMessage
    {
        public Guid Key { get; set; }

        public Guid BookingId { get; set; }

        public DateTime RejectedAt { get; set; }

    }
}

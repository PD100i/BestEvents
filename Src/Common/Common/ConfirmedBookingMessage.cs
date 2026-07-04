using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ConfirmedBookingMessage
    {
        public Guid BookingId { get; set; }

        public DateTime ConfirmedAt { get; set; }


    }
}

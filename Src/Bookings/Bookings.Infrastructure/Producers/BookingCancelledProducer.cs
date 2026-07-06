using Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookings.Infrastructure
{
    public class BookingCancelledProducer(ILogger<BookingCancelledProducer> logger) : BaseProducer<string, BookingCancelledMessage>("booking_cancelled", logger)
    {
    }
}

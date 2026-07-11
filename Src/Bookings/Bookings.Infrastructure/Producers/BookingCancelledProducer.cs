using Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookings.Infrastructure
{
    public class BookingCancelledProducer(IOptions<KafkaSettings> options, ILogger<BookingCancelledProducer> logger)
        : BaseProducer<Message>(options.Value, Topics.BookingCancelledTopic, logger)
    {
    }
}

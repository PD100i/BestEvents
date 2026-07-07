using Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Bookings.Infrastructure
{
    public class BookingCreatedProducer(ILogger<BookingCreatedProducer> logger) 
        : BaseProducer<BookingMessage>(Topics.BookingCreatedTopic, logger)
    {
    }
}

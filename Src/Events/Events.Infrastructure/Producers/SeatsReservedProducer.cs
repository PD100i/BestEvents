using Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure
{
    public class SeatsReservedProducer(ILogger<SeatsReservedProducer> logger) : BaseProducer<BookingMessage>(Topics.SeatsReservedTopic, logger)
    {
    }
}

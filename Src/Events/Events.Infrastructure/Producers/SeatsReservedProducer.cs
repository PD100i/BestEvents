using Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure
{
    public class SeatsReservedProducer(IOptions<KafkaSettings> options, ILogger<SeatsReservedProducer> logger) 
        : BaseProducer<Message>(options.Value, Topics.SeatsReservedTopic, logger)
    {
    }
}

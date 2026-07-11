using Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Events.Infrastructure
{
    public class SeatsReservationErrorProducer(IOptions<KafkaSettings> options, ILogger<SeatsReservationErrorProducer> logger)
        : BaseProducer<Message>(options.Value, Topics.SeatsReservationErrorTopic, logger)
    {
    }
}

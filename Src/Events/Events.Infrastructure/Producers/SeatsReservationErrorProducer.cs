using Common;
using Microsoft.Extensions.Logging;


namespace Events.Infrastructure
{
    public class SeatsReservationErrorProducer(ILogger<SeatsReservationErrorProducer> logger)
        : BaseProducer<BookingMessage>(Topics.SeatsReservationErrorTopic, logger)
    {
    }
}

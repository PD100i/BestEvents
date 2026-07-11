using Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Bookings.Infrastructure
{
    public class BookingCreatedProducer(IOptions<KafkaSettings> options, ILogger<BookingCreatedProducer> logger) 
        : BaseProducer<Message>(options.Value, Topics.BookingCreatedTopic, logger)
    {
    }
}

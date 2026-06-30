

namespace BestEvents.Application.Exceptions
{
    public class BookingLimitExceededException(string message) : Exception(message)
    {
    }
}

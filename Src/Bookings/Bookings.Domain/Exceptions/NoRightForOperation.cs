
namespace Bookings.Domain.Exceptions
{
    public class NoRightForOperation(string message) : Exception (message)
    {
    }
}

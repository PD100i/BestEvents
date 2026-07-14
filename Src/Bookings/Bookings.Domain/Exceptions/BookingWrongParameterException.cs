

namespace Bookings.Domain.Exceptions
{
    /// <summary>
    /// Выбрасывается при попытке бронирования с некорректными данными
    /// </summary>
    public class BookingWrongParameterException : Exception
    {
        public BookingWrongParameterException(string message) : base(message)
        {
        }
    }
}

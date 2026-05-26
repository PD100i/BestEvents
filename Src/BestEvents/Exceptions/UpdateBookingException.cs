namespace BestEvents.Exceptions
{
    /// <summary>
    /// Бросается при неудачном обновлении бронирования
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public class UpdateBookingException(string message, Exception exception) : Exception(message, exception)
    {
    }
}

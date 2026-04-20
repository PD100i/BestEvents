namespace BestEvents.Exceptions
{
    /// <summary>
    /// Выбрасывается при некорректном освобождении забронированных мест
    /// </summary>
    /// <param name="message"></param>
    public class ReleaseBookingException(string message) : Exception(message)
    {
    }
}

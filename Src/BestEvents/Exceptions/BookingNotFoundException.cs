namespace BestEvents.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое при попытке получить бронирование, которого нет в репозитории
    /// </summary>
    /// <param name="message"></param>
    public class BookingNotFoundException(string message) : Exception(message)
    {
    }
}

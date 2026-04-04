namespace BestEvents.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое при попытке передать сервису бронирования некорректные данные
    /// </summary>
    /// <param name="message"></param>
    public class BookingWrongParameterException(string message) : Exception(message)
    {
    }
}

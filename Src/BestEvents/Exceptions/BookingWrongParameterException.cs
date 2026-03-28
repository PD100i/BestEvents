namespace BestEvents.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое при попытке передать сервису бронирования некорректные данные (например, если дата создания брони в будущем)
    /// </summary>
    /// <param name="message"></param>
    public class BookingWrongParameterException(string message) : Exception(message)
    {
    }
}

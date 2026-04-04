namespace BestEvents.Exceptions
{
    /// <summary>
    /// Исключение, которое выбрасывается при ошибках, связанных с созданием бронирования.
    /// </summary>
    /// <param name="message"></param>
    public class CreateBookingException(string message) : Exception(message)
    {
    }
}

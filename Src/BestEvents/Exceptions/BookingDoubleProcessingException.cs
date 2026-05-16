namespace BestEvents.Exceptions
{
    /// <summary>
    /// Выбрасывается при повторной попытке перевести Booking в состояние Confirm или Reject
    /// </summary>
    /// <param name="message"></param>
    public class BookingDoubleProcessingException(string message) : Exception(message)
    {
    }
}

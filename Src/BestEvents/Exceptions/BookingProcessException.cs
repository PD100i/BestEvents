namespace BestEvents.Exceptions
{
    /// <summary>
    /// Выбрасывается при ошибках в процессе обработки брони
    /// </summary>
    /// <param name="message"></param>
    public class BookingProcessException(string message) : Exception(message) 
    {
    }

    
}

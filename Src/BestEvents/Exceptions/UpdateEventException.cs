namespace BestEvents.Exceptions
{
    /// <summary>
    /// Бросается при ошибке обновления события
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public class UpdateEventException(string message, Exception exception) : Exception(message, exception)
    {
    }
}

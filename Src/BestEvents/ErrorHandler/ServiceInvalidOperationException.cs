namespace BestEvents.ErrorHandler
{
    /// <summary>
    /// Исключение, которое выбрасывается при возникновение логических ошибок в сервисах
    /// </summary>
    /// <param name="message"></param>
    public class ServiceInvalidOperationException(string message) : InvalidOperationException(message)
    {
    }
}

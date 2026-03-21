namespace BestEvents.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое при попытке получить события, которых нет в репозитории
    /// </summary>
    /// <param name="message"></param>
    public class EventsNotFoundException(string message): Exception(message)
    {
    }
}

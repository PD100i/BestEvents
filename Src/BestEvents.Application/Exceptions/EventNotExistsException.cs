namespace BestEvents.Application.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое при попытке получить событие, которого нет в репозитории
    /// </summary>
    /// <param name="message"></param>
    public class EventNotExistsException(string message) : Exception(message)
    {
    }
}

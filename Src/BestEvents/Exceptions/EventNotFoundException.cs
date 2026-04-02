namespace BestEvents.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое при попытке получить событие, которого нет в репозитории
    /// </summary>
    /// <param name="message"></param>
    public class EventNotFoundException(string message): Exception(message)
    {
    }
}

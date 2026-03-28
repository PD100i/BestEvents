namespace BestEvents.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое при попытке получить данные, которых нет в репозитории
    /// </summary>
    /// <param name="message"></param>
    public class DataNotFoundException(string message): Exception(message)
    {
    }
}

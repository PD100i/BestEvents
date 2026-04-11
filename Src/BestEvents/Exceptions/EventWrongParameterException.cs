namespace BestEvents.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое при попытке передать сервису событий некорректные данные (например, если дата начала позже даты завершения)
    /// </summary>
    /// <param name="message"></param>
    public class EventWrongParameterException(string message): Exception(message)
    {
    }
}

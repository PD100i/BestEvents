namespace BestEvents.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое при попытке создать событие с некорректными данными (например, если дата начала позже даты завершения)
    /// </summary>
    /// <param name="message"></param>
    public class EventWrongParameterException(string message): ArgumentException(message)
    {
    }
}

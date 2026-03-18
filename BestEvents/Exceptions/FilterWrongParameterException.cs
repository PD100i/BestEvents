namespace BestEvents.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое при попытке фильтрации некорректными данными
    /// </summary>
    /// <param name="message"></param>
    public class FilterWrongParameterException(string message): Exception(message)
    {
    }
}

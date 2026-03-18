namespace BestEvents.Exceptions
{
    /// <summary>
    /// /// Исключение, выбрасываемое при попытке выполнить пагинацию с некорректными данными
    /// </summary>
    /// <param name="message"></param>
    public class PaginationWromgParameterException(string message): Exception(message)
    {
    }
}

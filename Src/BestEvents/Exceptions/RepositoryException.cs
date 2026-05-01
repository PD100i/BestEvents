namespace BestEvents.Exceptions
{
    /// <summary>
    /// Выбрасывается при возникновении ошибок, связанных с репозиторием, таких как проблемы с базой данных или ошибки доступа к данным.
    /// </summary>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="ex">Внутренняя ошибка.</param>
    public class RepositoryException(string message, Exception ex) : Exception(message, ex)
    {
       
    }
}

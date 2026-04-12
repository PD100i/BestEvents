namespace BestEvents.Exceptions
{
    /// <summary>
    /// Выбрасывется при попытке бронирования события, когда доступных мест нет
    /// </summary>
    public class NoAvailableSeatsException(): Exception(Messages_ru.NoAvailableSeats)
    {
    }
}

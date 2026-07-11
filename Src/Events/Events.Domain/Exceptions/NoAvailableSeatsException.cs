namespace Events.Domain.Exceptions
{
    /// <summary>
    /// Выбрасывется при попытке бронирования события, когда доступных мест нет
    /// </summary>
    public class NoAvailableSeatsException(): ReserveSeatsException(Messages_ru.NoAvailableSeats)
    {
    }
}

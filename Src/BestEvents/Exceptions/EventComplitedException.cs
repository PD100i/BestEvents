namespace BestEvents.Exceptions
{
    /// <summary>
    /// Выбрасывается при попытке забронировать событие, которое уже завершилось
    /// </summary>
    public class EventCompletedException() : Exception(Messages_ru.BookingRejectedEventCompleted)
    {
    }
}

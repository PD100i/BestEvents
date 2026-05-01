namespace BestEvents
{
    /// <summary>
    /// сущность события для хранения в базе данных
    /// </summary>
    public class EventEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название события
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// Описание события
        /// </summary>
        public string? Description { get; set; } = "";

        /// <summary>
        /// Время начала
        /// </summary>
        public DateTime StartAt { get; set; }

        /// <summary>
        /// Время завершения
        /// </summary>
        public DateTime EndAt { get; set; }

        /// <summary>
        /// Общее количество мест на событии
        /// </summary>
        public int TotalSeats { get; private set; }

        /// <summary>
        /// Текущее число свободных мест
        /// </summary>
        public int AvailableSeats { get; private set; }

        /// <summary>
        /// Список бронирований, связанных с этим событием
        /// </summary>
        public List<BookingEntity> Bookings { get; set; } = new List<BookingEntity>();

    }
}

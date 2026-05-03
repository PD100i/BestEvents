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
        public int TotalSeats { get; set; }

        /// <summary>
        /// Текущее число свободных мест
        /// </summary>
        public int AvailableSeats { get; set; }

        /// <summary>
        /// Список бронирований, связанных с этим событием
        /// </summary>
        public List<BookingEntity>? Bookings { get; set; } = null;


        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            EventEntity other = (EventEntity)obj;
            return Id == other.Id &&
                   Title == other.Title &&
                   Description == other.Description &&
                   StartAt == other.StartAt &&
                   EndAt == other.EndAt &&
                   TotalSeats == other.TotalSeats &&
                   AvailableSeats == other.AvailableSeats;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Description, StartAt, EndAt, TotalSeats, AvailableSeats);
        }

    }
}

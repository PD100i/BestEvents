using System.ComponentModel.DataAnnotations;
using BestEvents.Exceptions;

namespace BestEvents
{
    /// <summary>
    /// Модель события
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="title">Название события</param>
        /// <param name="startAt">Дата начала</param>
        /// <param name="endAt">Дата завершения</param>
        /// <param name="description">Описание (необязательный параметр)</param>
        /// <param name="totalSeats">Общее количество мест на событии</param>
        /// <param name="availableSeats">Доступное количество мест на событии</param>
        public Event(Guid id, string title, DateTime? startAt, DateTime? endAt, string? description, int totalSeats, int availableSeats)
        {
            Id = id;
            if (string.IsNullOrEmpty(title))
                throw new EventWrongParameterException(Messages_ru.No_Title);
            Title = title;
            if (startAt == default)
                throw new EventWrongParameterException(Messages_ru.No_StartAt);
            if (endAt == default)
                throw new EventWrongParameterException(Messages_ru.No_EndAt);
            if (startAt > endAt)
                throw new EventWrongParameterException(Messages_ru.EndAt_Less_StartAt);
            if (totalSeats <= 0)
                throw new EventWrongParameterException(Messages_ru.WrongEventTotalSeats);
            if (availableSeats <= 0 || availableSeats > totalSeats)
                throw new EventWrongParameterException(Messages_ru.WrongEventAvailableSeats);
            StartAt = startAt;
            EndAt = endAt;
            Description = description;
        }

        /// <summary>
        /// Конструктор без параметра id, который генерируется автоматически при создании события
        /// </summary>
        /// <param name="title">Название события</param>
        /// <param name="startAt">Дата начала</param>
        /// <param name="endAt">Дата завершения</param>
        /// <param name="description">Описание (необязательный параметр)</param>
        /// <param name="totalSeats">Общее количество мест на событии</param>
        public Event(string title, DateTime? startAt, DateTime? endAt, string? description, int totalSeats) :
            this(Guid.NewGuid(), title, startAt, endAt, description, totalSeats, totalSeats)
        {
        }


        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Название события
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Описание события
        /// </summary>
        public string? Description { get; } = "";

        /// <summary>
        /// Время начала
        /// </summary>
        public DateTime? StartAt { get; }

        /// <summary>
        /// Время завершения
        /// </summary>
        public DateTime? EndAt { get; }

        /// <summary>
        /// Общее количество мест на событии
        /// </summary>
        public int TotalSeats { get; }

        /// <summary>
        /// Текущее число свободных мест
        /// </summary>
        public int AvailableSeats { get; private set; }

        /// <summary>
        /// Возвращает false если свободных мест недостаточно, в противном случае, уменьшает AvailableSeats на count и возвращает true 
        /// </summary>
        /// <param name="count">Количество запрашиваемых для резервирования мест</param>
        /// <returns></returns>
        public bool TryReserveSeats(int count = 1)
        {
            if (AvailableSeats < count)
                return false;
            AvailableSeats -= count; 
            return true;
        }

        /// <summary>
        /// Освобождает count мест при освобождении брони, увеличивает AvalableSeats на count
        /// </summary>
        /// <param name="count"></param>
        public void ReleaseSeats(int count = 1)
        {
            if ((AvailableSeats + count) > TotalSeats)
                throw new ReleaseBookingException(Messages_ru.RealiseBokingWrongCount);
            AvailableSeats += count;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Event other = (Event)obj;
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

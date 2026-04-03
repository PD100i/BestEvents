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
        public Event(Guid id, string title, DateTime? startAt, DateTime? endAt, string? description)
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
        public Event(string title, DateTime? startAt, DateTime? endAt, string? description) :
            this(Guid.NewGuid(), title, startAt, endAt, description)
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
                   EndAt == other.EndAt;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Description, StartAt, EndAt);
        }

    }
}

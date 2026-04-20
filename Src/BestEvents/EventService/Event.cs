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
        /// Фабричный метод создания обекта event и заполнения всех свойств
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="title">Название события</param>
        /// <param name="startAt">Дата начала</param>
        /// <param name="endAt">Дата завершения</param>
        /// <param name="description">Описание (необязательный параметр)</param>
        /// <param name="totalSeats">Общее количество мест на событии</param>
        /// <param name="availableSeats">Доступное количество мест на событии</param>
        public static Event CreateEvent(Guid id, string title, DateTime? startAt, DateTime? endAt, string? description, int? totalSeats, int? availableSeats)
        {
           
            if (string.IsNullOrEmpty(title))
                throw new EventWrongParameterException(Messages_ru.No_Title);
           
            if (startAt == null)
                throw new EventWrongParameterException(Messages_ru.No_StartAt);
            if (endAt == null)
                throw new EventWrongParameterException(Messages_ru.No_EndAt);
            if (startAt > endAt)
                throw new EventWrongParameterException(Messages_ru.EndAt_Less_StartAt);
            if (totalSeats == null)
                throw new EventWrongParameterException(Messages_ru.No_TotalSeats);
            if (totalSeats < 1)
                throw new EventWrongParameterException(Messages_ru.WrongEventTotalSeats);
            if (availableSeats == null)
                throw new EventWrongParameterException(Messages_ru.No_AvailableSeats);
            if (availableSeats < 0 || availableSeats > totalSeats)
                throw new EventWrongParameterException(Messages_ru.WrongEventAvailableSeats);
            return new Event()
                    {
                        Id = id,
                        Title = title,
                        StartAt = startAt.Value,
                        EndAt = endAt.Value,
                        Description = description,
                        TotalSeats = totalSeats.Value,
                        AvailableSeats = availableSeats.Value
                    };        

        }

        /// <summary>
        /// Фабричный метод для создания нового события
        /// </summary>
        /// <param name="title">Название события</param>
        /// <param name="startAt">Дата начала</param>
        /// <param name="endAt">Дата завершения</param>
        /// <param name="description">Описание (необязательный параметр)</param>
        /// <param name="totalSeats">Общее количество мест на событии</param>
        public static Event CreateNewEvent(string title, DateTime? startAt, DateTime? endAt, string? description, int? totalSeats)
        {
            return CreateEvent(Guid.NewGuid(), title, startAt, endAt, description, totalSeats, totalSeats);
        }


        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Название события
        /// </summary>
        public string Title { get; private set; } = ""; 

        /// <summary>
        /// Описание события
        /// </summary>
        public string? Description { get; private set; } = "";

        /// <summary>
        /// Время начала
        /// </summary>
        public DateTime StartAt { get; private set; }

        /// <summary>
        /// Время завершения
        /// </summary>
        public DateTime EndAt { get; private set; }

        /// <summary>
        /// Общее количество мест на событии
        /// </summary>
        public int TotalSeats { get; private set; }

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

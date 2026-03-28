using BestEvents.Exceptions;

namespace BestEvents
{
    /// <summary>
    /// Модель бронирования, которая содержит информацию о бронировании события
    /// </summary>
    public class Booking
    {
        /// <summary>
        /// Создание брони для события с идентификатором eventId. Статус брони по умолчанию - Pending, дата создания - текущая дата и время
        /// </summary>
        /// <param name="eventId"></param>
        public Booking(Guid eventId): this(Guid.NewGuid(), eventId, BookingStatus.Pending, DateTime.Now)
        {
        }


        /// <summary>
        /// Создание брони для события с идентификатором eventId, статусом status и датой создания createdAt.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="status"></param>
        /// <param name="createdAt"></param>
        public Booking(Guid eventId, BookingStatus status, DateTime? createdAt) : this(Guid.NewGuid(), eventId, status, createdAt)
        {
        }

        /// <summary>
        /// Создание брони для события с идентификатором eventId, статусом status и датой создания createdAt. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventId"></param>
        /// <param name="status"></param>
        /// <param name="createdAt"></param>
        /// <exception cref="BookingWrongParameterException"></exception>
        public Booking(Guid id, Guid eventId, BookingStatus status, DateTime? createdAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("При создании брони не передан идентификатор брони");
            if (eventId == Guid.Empty)
                throw new ArgumentException("При создании брони не передан идентификатор события");
            if (createdAt == null)
                throw new ArgumentException("При создании брони не передана дата создания брони");
            if (createdAt > DateTime.Now)
                throw new ArgumentException("Дата создания брони не может быть в будущем");

            Id = id;
            EventId = eventId;
            Status = status;
            CreatedAt = createdAt.Value;
        }

        /// <summary>
        /// Идентификатор брони
        /// </summary>
        public Guid Id { get;  }

        /// <summary>
        /// Идентификатор события, на которое было сделано бронирование
        /// </summary>
        public Guid EventId { get; }

        /// <summary>
        /// Статус брони
        /// </summary>
        public BookingStatus Status { get; set; }

        /// <summary>
        /// Дата и время создания брони
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Дата и время обработки брони
        /// </summary>
        public DateTime? ProcessedAt { get; set; }
    }

    /// <summary>
    /// Перечисление статуса бронирования
    /// </summary>
    public enum BookingStatus
    {
        /// <summary>
        /// Бронь ожидает обработки
        /// </summary>
        Pending,
        /// <summary>
        /// Бронь подтверждена
        /// </summary>
        Confirmed,
        /// <summary>
        /// Бронь отклонена
        /// </summary>
        Rejected
    }
}

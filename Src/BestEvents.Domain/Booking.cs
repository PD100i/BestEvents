using BestEvents.Domain.Exceptions;


namespace BestEvents.Domain
{
    /// <summary>
    /// Модель бронирования, которая содержит информацию о бронировании события
    /// </summary>
    public class Booking
    {
        /// <summary>
        /// Конструктор для создания пустой брони
        /// </summary>
        public Booking() { }

        /// <summary>
        /// Создание брони для события с идентификатором eventId. Статус брони по умолчанию - Pending, дата создания - текущая дата и время
        /// </summary>
        /// <param name="id"></param>
        /// <param name="_event"></param>
        public Booking(Guid id, Event _event, Guid userId)
        {
            
            Id = id;
            EventId = _event.Id;
            Event = _event;
            Status = BookingStatus.Pending;
            CreatedAt = DateTime.Now;
            UserId = userId;
        }

        /// <summary>
        /// Идентификатор брони
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор события, на которое было сделано бронирование
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Событие, на которое сделано бронирование
        /// </summary>
        public Event? Event { get; set; }

        /// <summary>
        /// Идентификатор пользователя, зарезервировавшего событие
        /// </summary>
        public Guid UserId { get; set; }

      
        /// <summary>
        /// Статус брони
        /// </summary>
        public BookingStatus Status { get; set; }

        /// <summary>
        /// Дата и время создания брони
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата и время обработки брони
        /// </summary>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Подтверждение бронирования
        /// </summary>
        public void Confirm()
        {
            if (Status == BookingStatus.Confirmed)
                throw new BookingDoubleProcessingException(string.Format(Messages_ru.DoubleBookingProcessing, Id));
            if (Status == BookingStatus.Rejected)
                throw new BookingDoubleProcessingException(string.Format(Messages_ru.DoubleBookingProcessing, Id));
            if (Status == BookingStatus.Cancelled)
                throw new BookingDoubleProcessingException(string.Format(Messages_ru.DoubleBookingProcessing, Id));

            Status = BookingStatus.Confirmed;
            ProcessedAt = DateTime.Now;
        }

        /// <summary>
        /// Отклонение бронирования
        /// </summary>
        public void Reject()
        {
            if (Status == BookingStatus.Confirmed)
                throw new BookingDoubleProcessingException(string.Format(Messages_ru.DoubleBookingProcessing, Id));
            if (Status == BookingStatus.Rejected)
                throw new BookingDoubleProcessingException(string.Format(Messages_ru.DoubleBookingProcessing, Id));
            if (Status == BookingStatus.Cancelled)
                throw new BookingDoubleProcessingException(string.Format(Messages_ru.DoubleBookingProcessing, Id));
            Status = BookingStatus.Rejected;
            ProcessedAt = DateTime.Now;
        }

        public void Cancel(User user)
        {
            if (Status == BookingStatus.Confirmed)
                throw new BookingDoubleProcessingException(string.Format(Messages_ru.DoubleBookingProcessing, Id));
            if (Status == BookingStatus.Rejected)
                throw new BookingDoubleProcessingException(string.Format(Messages_ru.DoubleBookingProcessing, Id));
            if (Status == BookingStatus.Cancelled)
                throw new BookingDoubleProcessingException(string.Format(Messages_ru.DoubleBookingProcessing, Id));
            if (user.Id == UserId || user.Role == UserRolesEnum.Admin)
                Status = BookingStatus.Cancelled;
            else
                throw new NoRightOfCancelBookingException();
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Booking other = (Booking)obj;
            return Id == other.Id &&
                   EventId == other.EventId &&
                   CreatedAt == other.CreatedAt &&
                   ProcessedAt == other.ProcessedAt &&
                   Status == other.Status;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, EventId, CreatedAt, ProcessedAt, Status);
        }

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
        Rejected,
        /// <summary>
        /// Бронь отменена
        /// </summary>
        Cancelled
    }
}

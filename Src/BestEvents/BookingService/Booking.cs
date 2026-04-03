using BestEvents.ErrorHandler;
using BestEvents.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

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
        public Booking(Guid eventId)
        {
            if (eventId == Guid.Empty)
                throw new BookingWrongParameterException(Messages_ru.CreateBooking_No_EventId);

            Id = Guid.NewGuid();
            EventId = eventId;
            Status = BookingStatus.Pending;
            CreatedAt = DateTime.Now;
        }

        /// <summary>
        /// Идентификатор брони
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Идентификатор события, на которое было сделано бронирование
        /// </summary>
        public Guid EventId { get; }

        /// <summary>
        /// Статус брони
        /// </summary>
        public BookingStatus Status { get; private set; }

        /// <summary>
        /// Дата и время создания брони
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Дата и время обработки брони
        /// </summary>
        public DateTime? ProcessedAt { get; private set; }

        /// <summary>
        /// Подтверждение бронирования
        /// </summary>
        public void Confirm()
        {
            if (Status == BookingStatus.Confirmed)
                throw new ServiceInvalidOperationException(string.Format(Messages_ru.DoubleBookingConfirm, Id));
            if (Status == BookingStatus.Rejected)
                throw new ServiceInvalidOperationException(string.Format(Messages_ru.TryConfirmRejectedBooking, Id));
            Status = BookingStatus.Confirmed;
            ProcessedAt = DateTime.Now;
        }

        /// <summary>
        /// Отклонение бронирования
        /// </summary>
        public void Reject()
        {
            if (Status == BookingStatus.Confirmed)
                throw new ServiceInvalidOperationException(string.Format(Messages_ru.TryRedjectConfirmedBooking, Id));
            if (Status == BookingStatus.Rejected)
                throw new ServiceInvalidOperationException(string.Format(Messages_ru.DoubleBookingReject, Id));
            Status = BookingStatus.Rejected;
            ProcessedAt = DateTime.Now;
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
        Rejected
    }
}

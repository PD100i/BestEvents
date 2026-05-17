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
        /// Конструктор для создания пустой брони
        /// </summary>
        public Booking() { }

        /// <summary>
        /// Создание брони для события с идентификатором eventId. Статус брони по умолчанию - Pending, дата создания - текущая дата и время
        /// </summary>
        /// <param name="id"></param>
        /// <param name="_event"></param>
        public Booking(Guid id, Event _event)
        {
            
            Id = id;
            EventId = _event.Id;
            Event = _event;
            Status = BookingStatus.Pending;
            CreatedAt = DateTime.Now;
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
                throw new BookingDoubleProcessingException(string.Format(Messages_ru.DoubleBookingConfirm, Id));
            if (Status == BookingStatus.Rejected)
                throw new BookingDoubleProcessingException(string.Format(Messages_ru.TryConfirmRejectedBooking, Id));
            Status = BookingStatus.Confirmed;
            ProcessedAt = DateTime.Now;
        }

        /// <summary>
        /// Отклонение бронирования
        /// </summary>
        public void Reject()
        {
            if (Status == BookingStatus.Confirmed)
                throw new BookingDoubleProcessingException(string.Format(Messages_ru.TryRedjectConfirmedBooking, Id));
            if (Status == BookingStatus.Rejected)
                throw new BookingDoubleProcessingException(string.Format(Messages_ru.DoubleBookingReject, Id));
            Status = BookingStatus.Rejected;
            ProcessedAt = DateTime.Now;
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

        /// <summary>
        /// Фабричный метод для создания нового бронирования
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="_event"></param>
        /// <returns></returns>
        public static Booking CreateBooking(Guid bookingId, Event _event)
        {
            if (_event.EndAt < DateTime.UtcNow)
                throw new EventCompletedException();
            if (!_event.TryReserveSeats())
                throw new NoAvailableSeatsException();

            return new Booking(bookingId, _event);
        }

        /// <summary>
        /// Метод для подтверждения бронирования
        /// </summary>
        /// <param name="booking"></param>
        public static void Confirm(Booking booking)
        {
            if (booking.Event == null)
                throw new EventNotFoundException(string.Format(Messages_ru.CreateBookingEventNotFound, booking.EventId));
            if (booking.Event.EndAt < DateTime.UtcNow)
                throw new EventCompletedException();
            booking.Confirm();
        }

        /// <summary>
        /// Метод для отклонения бронирования
        /// </summary>
        /// <param name="booking"></param>
        public static void Reject(Booking booking)
        {
            booking.Reject();
            if (booking.Event != null)
                booking.Event.ReleaseSeats();
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

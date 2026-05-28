using BestEvents.Domain;

namespace BestEvents.Infrastructure
{
    /// <summary>
    /// Сущность бронирования для хранения в базе данных
    /// </summary>
    public class BookingEntity
    {
        /// <summary>
        /// Идентификатор брони
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор события, на которое было сделано бронирование
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Навигационное свойство для доступа к связанному событию
        /// </summary>
        public EventEntity? Event { get; set; }

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
    }
}

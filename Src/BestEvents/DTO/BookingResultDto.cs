
namespace BestEvents
{
    /// <summary>
    /// DTO для возврата результата бронирования события.
    /// </summary>
    public class BookingResultDto
    {
        /// <summary>
        /// Идентификатор брони
        /// </summary>
        public required string Id { get; set; } 

        /// <summary>
        /// Идентификатор события, на которое было сделано бронирование
        /// </summary>
        public required string EventId { get; set; } 

        /// <summary>
        /// Статус брони
        /// </summary>
        public required BookingStatus Status { get; set; }

        /// <summary>
        /// Дата и время создания брони
        /// </summary>
        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата и время обработки брони
        /// </summary>
        public required DateTime? ProcessedAt { get; set; }
    }
}

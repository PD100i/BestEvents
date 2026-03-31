using BestEvents.Exceptions;
using Microsoft.Extensions.Logging;

namespace BestEvents
{
    /// <summary>
    /// Реализация сервиса бронирования
    /// </summary>
    public class BookingService(IEventRepository eventsRepo, IBookingRepository bookingRepo) : IBookingService
    {
        /// <inheritdoc/>
        public static TimeSpan AllowedTimeUntilEventEnd { get; set; } = TimeSpan.FromHours(2);

        /// <inheritdoc/>
        public async Task<BookingResultDto> CreateBookingAsync(string eventId, CancellationToken ct)
        {
            Guid id = ParseStringId(eventId);
            Event _event = await (eventsRepo.GetEventAsync(id, ct));
            CheckEndAt(_event.EndAt);
            Booking booking = new(id);
            return new BookingResultDto
            {
                Id = booking.Id.ToString(),
                EventId = booking.EventId.ToString(),
                Status = booking.Status,
                CreatedAt = booking.CreatedAt,
                ProcessedAt = booking.ProcessedAt
            };
        }

        /// <inheritdoc/>
        public async Task<BookingResultDto> GetBookingByIdAsync(string bookingId, CancellationToken ct)
        {
            Guid id = ParseStringId(bookingId);
            Booking booking = await (bookingRepo.GetBookingAsync(id, ct));
            return new BookingResultDto
            {
                Id = booking.Id.ToString(),
                EventId = booking.EventId.ToString(),
                Status = booking.Status,
                CreatedAt = booking.CreatedAt,
                ProcessedAt = booking.ProcessedAt
            };
        }


        private Guid ParseStringId(string id)
        {
            if (!Guid.TryParse(id, out Guid result))
                throw new BookingWrongParameterException("Строка Id не соответствует формату GUID");
            return result;
        }

        private void CheckEndAt(DateTime endAt)
        {
            if (DateTime.Now > endAt - AllowedTimeUntilEventEnd)
                throw new BookingWrongParameterException($"Бронирование недоступно, так как до окончания события осталось меньше {AllowedTimeUntilEventEnd}");
        }
    }
}

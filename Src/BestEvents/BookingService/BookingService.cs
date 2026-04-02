using BestEvents.Exceptions;
using Microsoft.Extensions.Logging;

namespace BestEvents
{
    /// <summary>
    /// Реализация сервиса бронирования
    /// </summary>
    public class BookingService(IEventRepository eventRepo, IBookingRepository bookingRepo) : IBookingService
    {
        /// <inheritdoc/>
        public static TimeSpan AllowedTimeUntilEventEnd { get; set; } = TimeSpan.FromHours(2);

        /// <inheritdoc/>
        public async Task<BookingResultDto> CreateBookingAsync(string eventId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            Guid id = ParseStringId(eventId);
            Event _ = eventRepo.GetEvent(id);          
            Booking booking = await bookingRepo.CreateBookingAsync(new Booking(id), ct);
            return new BookingResultDto
            {
                Id = booking.Id.ToString(),
                EventId = booking.EventId.ToString(),
                Status = booking.Status.ToString(),
                CreatedAt = booking.CreatedAt,
                ProcessedAt = booking.ProcessedAt
            };
        }

        /// <inheritdoc/>
        public async Task<BookingResultDto> GetBookingByIdAsync(string bookingId, CancellationToken ct)
        {
            Guid id = ParseStringId(bookingId);
            ct.ThrowIfCancellationRequested();
            Booking booking = await (bookingRepo.GetBookingAsync(id, ct));
            return new BookingResultDto
            {
                Id = booking.Id.ToString(),
                EventId = booking.EventId.ToString(),
                Status = booking.Status.ToString(),
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

    }
}

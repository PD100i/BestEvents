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
            Event _event = GetEvent(id);
            ValidateEvent(_event);
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
                throw new BookingWrongParameterException(Messages_ru.Wrong_Id_Format);
            return result;
        }
        private void ValidateEvent(Event _event)
        {
            if (_event.EndAt < DateTime.Now)
                throw new CreateBookingException(string.Format(Messages_ru.CreateBookingEventCompleted, _event.Id));
        }

        private Event GetEvent(Guid id)
        {
            try
            {
                return eventRepo.GetEvent(id);
            }
            catch (EventNotFoundException)
            {
                throw new BookingWrongParameterException(string.Format(Messages_ru.CreateBookingEventNotFound, id));
            }
        }
    }
}

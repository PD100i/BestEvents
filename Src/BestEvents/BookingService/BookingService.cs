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
            SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
            try
            {
                await semaphore.WaitAsync();
                Event _event = await GetEventAsync(id);
                await TryBookingAndUpdateEventAsync(_event);            
            }
            finally 
            { 
                semaphore.Release();
            }
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

        private static Guid ParseStringId(string id)
        {
            if (!Guid.TryParse(id, out Guid result))
                throw new BookingWrongParameterException(Messages_ru.Wrong_Id_Format);
            return result;
        }

        private async Task TryBookingAndUpdateEventAsync(Event _event)
        {
            if (_event.EndAt < DateTime.Now)
                throw new EventCompletedException();
            if (!_event.TryReserveSeats())
                throw new NoAvailableSeatsException();
            await eventRepo.ReplaceEventAsync(_event);
            return;
        }

        private async Task<Event> GetEventAsync(Guid id)
        {
            var _event = await eventRepo.GetEventAsync(id);
            if (_event == null)
                throw new EventNotFoundException(string.Format(Messages_ru.CreateBookingEventNotFound, id));
            return _event;
        }
    }
}

using BestEvents;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEventsTest
{
    public class BookingServiceFixture
    {
        
        public Mock<IEventRepository> MockEventRepository { get; set; }

        public Mock<IBookingRepository> MockBookingRepository { get; set; }

        public BookingService BookingService { get; set; }


        public BookingServiceFixture()
        {
            MockEventRepository = new Mock<IEventRepository>();
            MockBookingRepository = new Mock<IBookingRepository>();
            BookingService = new BookingService(MockEventRepository.Object, MockBookingRepository.Object);
        }
    }
    public class BookingServiceTest
    {
        [Fact]
        public async Task CreateBookingAsync_EventExists_ReturnBookingResultDto()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            const int TOTAL_SEATS = 10;
            const int AVAILABLE_SEATS = 8;
            var eventId = Guid.NewGuid();
            var startAt = DateTime.Now.AddDays(1);
            var endAt = DateTime.Now.AddDays(2);
            var _event = Event.CreateEvent(eventId, "Title", startAt, endAt, "", TOTAL_SEATS, AVAILABLE_SEATS);
            var modifiedEvent = Event.CreateEvent(eventId, "Title", startAt, endAt, "", TOTAL_SEATS, AVAILABLE_SEATS - 1);
            var booking = new Booking(eventId);

            fixture.MockEventRepository.Setup(repo => repo.GetEventAsync(eventId)).ReturnsAsync(_event);
            fixture.MockBookingRepository.Setup(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            // Act
            var result = await fixture.BookingService.CreateBookingAsync(eventId.ToString(), CancellationToken.None);

            // Assert
            fixture.MockEventRepository.Verify(repo => repo.GetEventAsync(eventId), Times.Once);
            fixture.MockEventRepository.Verify(repo => repo.ReplaceEventAsync(modifiedEvent), Times.Once);
            fixture.MockBookingRepository.Verify(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(booking.Id.ToString(), result.Id);
            Assert.Equal(booking.EventId.ToString(), result.EventId);
            Assert.Equal(booking.Status.ToString(), result.Status);
            Assert.Equal(booking.CreatedAt, result.CreatedAt);
            Assert.Equal(eventId.ToString(), result.EventId);
            Assert.Equal(BookingStatus.Pending.ToString(), result.Status);
            Assert.Null(result.ProcessedAt);
        }

        [Fact]
        public async Task CreateBookingAsync_MultipleRequestes_CreateSomeBooking()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            const int TOTAL_SEATS = 10;
            const int AVAILABLE_SEATS = 8;
            var eventId = Guid.NewGuid();
            var startAt = DateTime.Now.AddDays(1);
            var endAt = DateTime.Now.AddDays(2);
            var _event = Event.CreateEvent(eventId, "Title", startAt, endAt, "", TOTAL_SEATS, AVAILABLE_SEATS);

            fixture.MockEventRepository.Setup(repo => repo.GetEventAsync(eventId)).ReturnsAsync(_event);
            fixture.MockBookingRepository.Setup(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), CancellationToken.None))
               .ReturnsAsync((Booking booking, CancellationToken ct) => booking);

            List<Task<BookingResultDto>> t = [
                fixture.BookingService.CreateBookingAsync(eventId.ToString(), CancellationToken.None),
                fixture.BookingService.CreateBookingAsync(eventId.ToString(), CancellationToken.None),
                fixture.BookingService.CreateBookingAsync(eventId.ToString(), CancellationToken.None)];

            // Act
            var result = await Task.WhenAll(t);

            // Assert
            Assert.Equal(eventId.ToString(), result[0].EventId);
            Assert.Null(result[0].ProcessedAt);
            Assert.Equal(BookingStatus.Pending.ToString(), result[0].Status);

            Assert.Equal(eventId.ToString(), result[1].EventId);
            Assert.Null(result[1].ProcessedAt);
            Assert.Equal(BookingStatus.Pending.ToString(), result[1].Status);

            Assert.Equal(eventId.ToString(), result[2].EventId);
            Assert.Null(result[2].ProcessedAt);
            Assert.Equal(BookingStatus.Pending.ToString(), result[2].Status);

            Assert.NotEqual(result[0].Id, result[1].Id);
            Assert.NotEqual(result[1].Id, result[2].Id);

        }

        [Fact]
        public async Task CreateBookingAsync_Overbooking_CreateOnlyAvailableBookings()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            const int TOTAL_SEATS = 5;
            const int AVAILABLE_SEATS = 5;
            const int BOOKING_ATTEMPT = 15;
            var eventId = Guid.NewGuid();
            var startAt = DateTime.Now.AddDays(1);
            var endAt = DateTime.Now.AddDays(2);
            var _event = Event.CreateEvent(eventId, "Title", startAt, endAt, "", TOTAL_SEATS, AVAILABLE_SEATS);

            fixture.MockEventRepository.Setup(repo => repo.GetEventAsync(eventId)).ReturnsAsync(_event);
            fixture.MockBookingRepository.Setup(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), CancellationToken.None))
               .ReturnsAsync((Booking booking, CancellationToken ct) => booking);

            Task<BookingResultDto>[] tasks = new Task<BookingResultDto>[BOOKING_ATTEMPT]; 
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = fixture.BookingService.CreateBookingAsync(eventId.ToString(), CancellationToken.None);
            }

            // Act
            await Assert.ThrowsAsync<BestEvents.Exceptions.NoAvailableSeatsException>(async() => {var results = await Task.WhenAll(tasks); });

            // Assert
            fixture.MockEventRepository.Verify(repo => repo.GetEventAsync(eventId), Times.Exactly(BOOKING_ATTEMPT));
            fixture.MockBookingRepository.Verify(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Exactly(AVAILABLE_SEATS));
        }

        [Fact]
        public async Task CreateBookingAsync_EnoughSeats_CreateUnicBookingsWithSameEventId()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            const int TOTAL_SEATS = 10;
            const int AVAILABLE_SEATS = 10;
            var eventId = Guid.NewGuid();
            var startAt = DateTime.Now.AddDays(1);
            var endAt = DateTime.Now.AddDays(2);
            var _event = Event.CreateEvent(eventId, "Title", startAt, endAt, "", TOTAL_SEATS, AVAILABLE_SEATS);

            fixture.MockEventRepository.Setup(repo => repo.GetEventAsync(eventId)).ReturnsAsync(_event);
            fixture.MockBookingRepository.Setup(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), CancellationToken.None))
               .ReturnsAsync((Booking booking, CancellationToken ct) => booking);

            Task<BookingResultDto>[] tasks = new Task<BookingResultDto>[AVAILABLE_SEATS];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = fixture.BookingService.CreateBookingAsync(eventId.ToString(), CancellationToken.None);
            }

            // Act
            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(0, _event.AvailableSeats);
            Assert.Equal(AVAILABLE_SEATS, results.Select(r => r.Id).Distinct().Count());
            Assert.True(results.All(r => r.EventId == _event.Id.ToString()));
        }

        [Fact]
        public async Task CreateBookingAsync_EventDoesNotExist_ShouldThrowBookingNotFoundException()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var eventId = Guid.NewGuid();
            fixture.MockEventRepository.Setup(repo => repo.GetEventAsync(eventId)).ReturnsAsync(() => null);

            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.EventNotFoundException>(() => fixture.BookingService.CreateBookingAsync(eventId.ToString(), CancellationToken.None));
            fixture.MockEventRepository.Verify(repo => repo.GetEventAsync(eventId), Times.Once);
            fixture.MockBookingRepository.Verify(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_NoAwailableSeats_ShouldThrowNoAvailableSeatsException()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            const int TOTAL_SEATS = 10;
            const int AVAILABLE_SEATS = 0;
            var eventId = Guid.NewGuid();
            var startAt = DateTime.Now.AddDays(1);
            var endAt = DateTime.Now.AddDays(2);
            var _event = Event.CreateEvent(eventId, "Title", startAt, endAt, "", TOTAL_SEATS, AVAILABLE_SEATS);
            fixture.MockEventRepository.Setup(repo => repo.GetEventAsync(eventId)).ReturnsAsync(_event);

            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.NoAvailableSeatsException>(() => fixture.BookingService.CreateBookingAsync(eventId.ToString(), CancellationToken.None));
            fixture.MockEventRepository.Verify(repo => repo.GetEventAsync(eventId), Times.Once);
            fixture.MockBookingRepository.Verify(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_EventCompleted_ShouldThrowEventCompletedException()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var eventId = Guid.NewGuid();
            fixture.MockEventRepository.Setup(repo => repo.GetEventAsync(eventId)).ReturnsAsync(Event.CreateEvent(eventId, "Title", DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-1), "", 10, 8));

            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.EventCompletedException>(() => fixture.BookingService.CreateBookingAsync(eventId.ToString(), CancellationToken.None));
            fixture.MockEventRepository.Verify(repo => repo.GetEventAsync(eventId), Times.Once);
            fixture.MockBookingRepository.Verify(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_WrongFormatEventId_ShouldThrowBookingWrongParameterException()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var invalidEventId = "123";

            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.BookingWrongParameterException>(() => fixture.BookingService.CreateBookingAsync(invalidEventId, CancellationToken.None));
            fixture.MockEventRepository.Verify(repo => repo.GetEventAsync(It.IsAny<Guid>()), Times.Never);
            fixture.MockBookingRepository.Verify(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_Cancel_Cancelled()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var eventId = Guid.NewGuid();
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => fixture.BookingService.CreateBookingAsync(eventId.ToString(), cts.Token));
            fixture.MockEventRepository.Verify(repo => repo.GetEventAsync(It.IsAny<Guid>()), Times.Never);
            fixture.MockBookingRepository.Verify(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        public async Task GetBookingByIdAsync_BookingExists_ShouldReturnBookingResultDto()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var eventId = Guid.NewGuid();
            var booking = new Booking(eventId);
            fixture.MockBookingRepository.Setup(repo => repo.GetBookingAsync(booking.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            // Act
            var result = await fixture.BookingService.GetBookingByIdAsync(booking.Id.ToString(), CancellationToken.None);

            // Assert
            fixture.MockBookingRepository.Verify(repo => repo.GetBookingAsync(booking.Id, It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(booking.Id.ToString(), result.Id);
            Assert.Equal(booking.EventId.ToString(), result.EventId);
            Assert.Equal(booking.Status.ToString(), result.Status);
            Assert.Equal(booking.CreatedAt, result.CreatedAt);
        }

        [Fact]
        public async Task GetBookingByIdAsync_BookingDoesNotExist_ShouldThrowDataNotFoundException()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var bookingId = Guid.NewGuid();
            fixture.MockBookingRepository.Setup(repo => repo.GetBookingAsync(bookingId, It.IsAny<CancellationToken>()))
                .Throws(new BestEvents.Exceptions.EventNotFoundException("Бронирование не найдено"));

            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.EventNotFoundException>(() => fixture.BookingService.GetBookingByIdAsync(bookingId.ToString(), CancellationToken.None));
            fixture.MockBookingRepository.Verify(repo => repo.GetBookingAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetBookingByIdAsync_WrongFormatBookingId_ShouldThrowBookingWrongParameterException()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            string invalidBookingId = "123";
            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.BookingWrongParameterException>(() => fixture.BookingService.GetBookingByIdAsync(invalidBookingId, CancellationToken.None));
            fixture.MockBookingRepository.Verify(repo => repo.GetBookingAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetBookingByIdAsync_Cancel_Cancelled()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var bookingId = Guid.NewGuid();
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => fixture.BookingService.GetBookingByIdAsync(bookingId.ToString(), cts.Token));
            fixture.MockBookingRepository.Verify(repo => repo.GetBookingAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}

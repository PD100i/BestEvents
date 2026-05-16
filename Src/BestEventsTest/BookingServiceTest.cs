using BestEvents;
using BestEvents.Exceptions;
using Moq;


namespace BestEventsTest
{
    public class BookingServiceTest
    {
        private Booking CreateConfirmedBooking()
        {
            var _event = CreateEvent();

            return new Booking()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddSeconds(-4),
                Status = BookingStatus.Confirmed,
                ProcessedAt = DateTime.UtcNow.AddSeconds(-2),
                EventId = _event.Id,
                Event = _event
            };
        }

        private Event CreateEvent()
        {
            return new Event()
            {
                Id = Guid.NewGuid(),
                Title = "Event_1",
                StartAt = DateTime.UtcNow.AddDays(-5),
                EndAt = DateTime.UtcNow.AddDays(5),
                TotalSeats = 1000,
                AvailableSeats = 100
            };
        }

        [Fact]
        public async Task GetBookingAsync_ShouldCallGetBookingRepoReturnBooking()
        {
            // Arrange
            var booking = CreateConfirmedBooking();
            var mockRepo = new Mock<IBookingRepository>();
            mockRepo.Setup(repo => repo.GetBookingAsync(booking.Id, CancellationToken.None)).ReturnsAsync(() => booking);
            var bookingService = new BookingService(mockRepo.Object);

            // Act
            var result = await bookingService.GetBookingAsync(booking.Id, CancellationToken.None);

            // Assert
            Assert.Equal(booking, result);
            mockRepo.Verify(repo => repo.GetBookingAsync(booking.Id, CancellationToken.None), Times.Once());

        }

        [Fact]
        public async Task CreateBookingAsync_GoodCase_ReturnBooking()
        {
            // Arrange
            var _event = CreateEvent();
            var mockRepo = new Mock<IBookingRepository>();
            int expectedAvailableSeats = _event.AvailableSeats - 1;
            var bookingService = new BookingService(mockRepo.Object);

            mockRepo.Setup(repo => repo.AddBookingAsync(_event.Id, BookingService.CreateBookingAction, It.IsAny<CancellationToken>()))
                .Returns(() => BookingService.CreateBookingAction(_event, CancellationToken.None));

            // Act
            var booking = await bookingService.CreateBookingAsync(_event.Id, CancellationToken.None);

            // Assert
            Assert.Equal(booking.EventId, _event.Id);
            Assert.NotNull(booking.Event);
            Assert.Equal(booking.Event.AvailableSeats, expectedAvailableSeats);
            mockRepo.Verify(repo => repo.AddBookingAsync(_event.Id, BookingService.CreateBookingAction, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task CreateBookingAsync_EventCompleted_ReturnBooking()
        {
            // Arrange
            var _event = CreateEvent();
            _event.EndAt = DateTime.UtcNow.AddDays(-2);
            int expectedAvailableSeats = _event.AvailableSeats;

            var mockRepo = new Mock<IBookingRepository>();

            var bookingService = new BookingService(mockRepo.Object);

            mockRepo.Setup(repo => repo.AddBookingAsync(_event.Id, BookingService.CreateBookingAction, It.IsAny<CancellationToken>()))
                .Returns(() => BookingService.CreateBookingAction(_event, CancellationToken.None));

            // Act & Assert
            await Assert.ThrowsAsync<EventCompletedException>(() => bookingService.CreateBookingAsync(_event.Id, CancellationToken.None));
            mockRepo.Verify(repo => repo.AddBookingAsync(_event.Id, BookingService.CreateBookingAction, It.IsAny<CancellationToken>()), Times.Once());
            Assert.Equal(_event.AvailableSeats, expectedAvailableSeats);
        }

        [Fact]
        public async Task CreateBookingAsync_NoAvailableSeats_ReturnBooking()
        {
            // Arrange
            var _event = CreateEvent();
            int expectedAvailableSeats = 0;
            _event.AvailableSeats = 0;

            var mockRepo = new Mock<IBookingRepository>();

            var bookingService = new BookingService(mockRepo.Object);

            mockRepo.Setup(repo => repo.AddBookingAsync(_event.Id, BookingService.CreateBookingAction, It.IsAny<CancellationToken>()))
                .Returns(() => BookingService.CreateBookingAction(_event, CancellationToken.None));

            // Act & Assert
            await Assert.ThrowsAsync<NoAvailableSeatsException>(() => bookingService.CreateBookingAsync(_event.Id, CancellationToken.None));
            mockRepo.Verify(repo => repo.AddBookingAsync(_event.Id, BookingService.CreateBookingAction, It.IsAny<CancellationToken>()), Times.Once());
            Assert.Equal(_event.AvailableSeats, expectedAvailableSeats);
        }

        [Fact]
        public async Task TryProcessBooking_GoodCase_ShouldConfirmBooking()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            var booking = new Booking(bookingId, _event);
            var mockRepo = new Mock<IBookingRepository>();
            mockRepo.Setup(repo => repo.GetBookingAsync(bookingId, CancellationToken.None)).ReturnsAsync(() => booking);
            var bookingService = new BookingService(mockRepo.Object);
            
            // Act
            await bookingService.TryProcessBooking(bookingId, CancellationToken.None);

            // Assert
            mockRepo.Verify(repo => repo.GetBookingAsync(bookingId,CancellationToken.None), Times.Once());
            mockRepo.Verify(repo => repo.UpdateBookingAsync(It.Is<Booking>(b => b.Id ==  bookingId &&
                                                                                b.EventId == booking.EventId &&
                                                                                b.Event == booking.Event &&
                                                                                b.CreatedAt == booking.CreatedAt &&
                                                                                b.Status == BookingStatus.Confirmed &&
                                                                                b.ProcessedAt > b.CreatedAt), CancellationToken.None));
        }

        [Fact]
        public async Task TryProcessBooking_BookingNotFound_ShoultThrowBookingNotFoundException()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var mockRepo = new Mock<IBookingRepository>();
            mockRepo.Setup(repo => repo.GetBookingAsync(bookingId, CancellationToken.None)).ThrowsAsync(new BookingNotFoundException(Messages_ru.BookingNotFound));
            var bookingService = new BookingService(mockRepo.Object);

            // Act && Assert
            await Assert.ThrowsAsync<BookingNotFoundException>(() =>  bookingService.TryProcessBooking(bookingId, CancellationToken.None));
            mockRepo.Verify(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task TryProcessBooking_BookingAlreadyProcessed_ShouldDoubleBookingProcessingException()
        {
            // Arrange
            var booking = CreateConfirmedBooking();
            var mockRepo = new Mock<IBookingRepository>();
            mockRepo.Setup(repo => repo.GetBookingAsync(booking.Id, CancellationToken.None)).ThrowsAsync(new BookingDoubleProcessingException(Messages_ru.DoubleBookingConfirm));
            var bookingService = new BookingService(mockRepo.Object);

            // Act && Assert
            await Assert.ThrowsAsync<BookingDoubleProcessingException>(() => bookingService.TryProcessBooking(booking.Id, CancellationToken.None));
            mockRepo.Verify(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task TryProcessBooking_EventNotFound_ShoultRejectBookingAndThrowBookingNotFoundException()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var booking = new Booking()
            {
                Id = bookingId,
                CreatedAt = DateTime.UtcNow.AddSeconds(-4),
                Status = BookingStatus.Pending,
                ProcessedAt = null,
                EventId = Guid.NewGuid(),
                Event = null
            };
            var mockRepo = new Mock<IBookingRepository>();
            mockRepo.Setup(repo => repo.GetBookingAsync(bookingId, CancellationToken.None)).ReturnsAsync(() => booking);
            var bookingService = new BookingService(mockRepo.Object);

            // Act && Assert
            await Assert.ThrowsAsync<EventNotFoundException>(() => bookingService.TryProcessBooking(bookingId, CancellationToken.None));
            mockRepo.Verify(repo => repo.UpdateBookingAsync(It.Is<Booking>(b => b.Id == bookingId &&
                                                                                 b.Status == BookingStatus.Rejected),
                                                                                 It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task TryProcessBooking_EventCompleted_ShoultRejectBookingAndIncrementEventAwailableSeats()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            _event.EndAt = DateTime.UtcNow.AddDays(-2);
            int expectedAvalableSeats = _event.AvailableSeats + 1;
            var booking = new Booking(bookingId, _event);
            var mockRepo = new Mock<IBookingRepository>();
            mockRepo.Setup(repo => repo.GetBookingAsync(bookingId, CancellationToken.None)).ReturnsAsync(() => booking);
            var bookingService = new BookingService(mockRepo.Object);

            // Act && Assert
            await Assert.ThrowsAsync<EventCompletedException>(() => bookingService.TryProcessBooking(bookingId, CancellationToken.None));
            mockRepo.Verify(repo => repo.UpdateBookingAsync(It.Is<Booking>(b => b.Id == bookingId &&
                                                                                b.Status == BookingStatus.Rejected &&
                                                                                b.Event != null &&
                                                                                b.Event.AvailableSeats == expectedAvalableSeats),
                                                                                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

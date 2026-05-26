using BestEvents;
using BestEvents.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;
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
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();

            mockBookingRepo.Setup(repo => repo.GetBookingAsync(booking.Id, CancellationToken.None)).ReturnsAsync(() => booking);
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUow.Object);

            // Act
            var result = await bookingService.GetBookingAsync(booking.Id, CancellationToken.None);

            // Assert
            Assert.Equal(booking, result);
            mockBookingRepo.Verify(repo => repo.GetBookingAsync(booking.Id, CancellationToken.None), Times.Once());

        }


        [Fact]
        public async Task GetPandingBookings_ShouldCallGetPendingBookingsRepoReturnBookings()
        {
            // Assert
            List<Guid> bookings = [];
            int expectedCount = 10;
            for (int i = 0; i < expectedCount; i++)
            {
                bookings.Add(Guid.NewGuid());
            }
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            mockBookingRepo.Setup(repo => repo.GetPendingBookingsAsync(CancellationToken.None)).ReturnsAsync(() => bookings);
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUow.Object);

            // Act
            var result = await bookingService.GetPendingBookingsAsync(CancellationToken.None);

            // Assert
            Assert.Equal(bookings, result);
            mockBookingRepo.Verify(repo => repo.GetPendingBookingsAsync(CancellationToken.None), Times.Once());
            
        }

        [Fact]
        public async Task CreateBookingAsync_GoodCase_ReturnBooking()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockTransaction = new Mock<IDbContextTransaction>();
            int expectedAvailableSeats = _event.AvailableSeats - 1;
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUow.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockEventRepo.Setup(repo => repo.GetEventForUpdateAsync(_event.Id, CancellationToken.None)).ReturnsAsync(() => _event);
            mockBookingRepo.Setup(repo => repo.AddBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()));

            // Act
            var result = await bookingService.CreateBookingAsync(_event.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.NotNull(result.Event);
            Assert.Equal(result.Event.AvailableSeats, expectedAvailableSeats);
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.CommitAsync(CancellationToken.None), Times.Once());
            mockEventRepo.Verify(repo => repo.GetEventForUpdateAsync(_event.Id, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.AddBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once());
        }


        [Fact]
        public async Task CreateBookingAsync_EventCompleted_ReturnBooking()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            _event.EndAt = DateTime.UtcNow.AddDays(-2);
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockTransaction = new Mock<IDbContextTransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUow.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockEventRepo.Setup(repo => repo.GetEventForUpdateAsync(_event.Id, CancellationToken.None)).ReturnsAsync(() => _event);
            mockBookingRepo.Setup(repo => repo.AddBookingAsync(It.Is<Booking>(b => b.Id == bookingId), It.IsAny<CancellationToken>()));

            // Act & Assert
            await Assert.ThrowsAsync<EventCompletedException>(() => bookingService.CreateBookingAsync(_event.Id, CancellationToken.None));
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.Commit(), Times.Never());
            mockEventRepo.Verify(repo => repo.GetEventForUpdateAsync(_event.Id, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.AddBookingAsync(It.Is<Booking>(b => b.Id == bookingId), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task CreateBookingAsync_NoAvailableSeats_ReturnBooking()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            _event.AvailableSeats = 0;

            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockTransaction = new Mock<IDbContextTransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUow.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockEventRepo.Setup(repo => repo.GetEventForUpdateAsync(_event.Id, CancellationToken.None)).ReturnsAsync(() => _event);
            mockBookingRepo.Setup(repo => repo.AddBookingAsync(It.Is<Booking>(b => b.Id == bookingId), It.IsAny<CancellationToken>()));

            // Act & Assert
            await Assert.ThrowsAsync<NoAvailableSeatsException>(() => bookingService.CreateBookingAsync(_event.Id, CancellationToken.None));
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.Commit(), Times.Never());
            mockEventRepo.Verify(repo => repo.GetEventForUpdateAsync(_event.Id, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.AddBookingAsync(It.Is<Booking>(b => b.Id == bookingId), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task TryProcessBooking_GoodCase_ShouldConfirmBooking()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            var booking = new Booking(bookingId, _event);
            int expectedAvailableSeate = _event.AvailableSeats;

            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockTransaction = new Mock<IDbContextTransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUow.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockBookingRepo.Setup(repo => repo.GetBookingForUpdateAsync(bookingId, CancellationToken.None)).ReturnsAsync(booking);
            mockBookingRepo.Setup(repo => repo.UpdateBookingAsync(It.Is<Booking>(b => b.Id == bookingId &&
                                                                                      b.Status == BookingStatus.Confirmed &&
                                                                                      b.Event != null &&
                                                                                      b.Event.AvailableSeats == expectedAvailableSeate), 
                                                                                      CancellationToken.None));

            // Act
            await bookingService.TryProcessBooking(bookingId, CancellationToken.None);

            // Assert
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.CommitAsync(CancellationToken.None), Times.Once());
            mockBookingRepo.Verify(repo => repo.GetBookingForUpdateAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.UpdateBookingAsync(It.Is<Booking>(b => b.Id == bookingId &&
                                                                                      b.Status == BookingStatus.Confirmed &&
                                                                                      b.Event != null &&
                                                                                      b.Event.AvailableSeats == expectedAvailableSeate), 
                                                                                      It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task TryProcessBooking_BookingNotFound_ShoultThrowBookingNotFoundException()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockTransaction = new Mock<IDbContextTransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUow.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockBookingRepo.Setup(repo => repo.GetBookingForUpdateAsync(bookingId, CancellationToken.None)).ThrowsAsync(new BookingNotFoundException(Messages_ru.BookingNotFound));
            mockBookingRepo.Setup(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(), CancellationToken.None));

            // Act && Assert
            await Assert.ThrowsAsync<BookingNotFoundException>(() =>  bookingService.TryProcessBooking(bookingId, CancellationToken.None));
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.Commit(), Times.Never());
            mockBookingRepo.Verify(repo => repo.GetBookingForUpdateAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task TryProcessBooking_BookingAlreadyProcessed_ShouldDoubleBookingProcessingException()
        {
            // Arrange
            var booking = CreateConfirmedBooking();
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockTransaction = new Mock<IDbContextTransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUow.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockBookingRepo.Setup(repo => repo.GetBookingForUpdateAsync(booking.Id, CancellationToken.None)).ReturnsAsync(booking);
            mockBookingRepo.Setup(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(), CancellationToken.None));

            // Act && Assert
            await Assert.ThrowsAsync<BookingDoubleProcessingException>(() => bookingService.TryProcessBooking(booking.Id, CancellationToken.None));
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.Commit(), Times.Never());
            mockBookingRepo.Verify(repo => repo.GetBookingForUpdateAsync(booking.Id, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never());
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
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockTransaction = new Mock<IDbContextTransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUow.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockBookingRepo.Setup(repo => repo.GetBookingForUpdateAsync(bookingId, CancellationToken.None)).ThrowsAsync(new EventNotFoundException(Messages_ru.EventNotFound));
            mockBookingRepo.Setup(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(), CancellationToken.None));

            // Act && Assert
            await Assert.ThrowsAsync<EventNotFoundException>(() => bookingService.TryProcessBooking(bookingId, CancellationToken.None));
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.Commit(), Times.Never());
            mockBookingRepo.Verify(repo => repo.GetBookingForUpdateAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never());
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
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockTransaction = new Mock<IDbContextTransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUow.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockBookingRepo.Setup(repo => repo.GetBookingForUpdateAsync(bookingId, CancellationToken.None)).ReturnsAsync(booking);
            mockBookingRepo.Setup(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(), CancellationToken.None));

            // Act && Assert
            await Assert.ThrowsAsync<EventCompletedException>(() => bookingService.TryProcessBooking(bookingId, CancellationToken.None));
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.CommitAsync(CancellationToken.None), Times.Once());
            mockBookingRepo.Verify(repo => repo.GetBookingForUpdateAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once());
            Assert.Equal(BookingStatus.Rejected, booking.Status);
            Assert.NotNull(booking.Event);
            Assert.Equal(booking.Event.AvailableSeats, expectedAvalableSeats);
        }
    }
}

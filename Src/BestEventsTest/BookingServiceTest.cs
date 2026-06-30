using BestEvents.Application;
using BestEvents.Application.Exceptions;
using BestEvents.Domain;
using BestEvents.Domain.Exceptions;
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

        private User CreateUser()
        {
            return User.CreateUser(Guid.NewGuid(), "TestUser", "hashedpassword", "User");
        }

        private Event CreateEvent()
        {
            return new Event()
            {
                Id = Guid.NewGuid(),
                Title = "Event_1",
                StartAt = DateTime.UtcNow.AddDays(3),
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
            var mockUserRepo = new Mock<IUserRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockUserAccessor = new Mock<IUserAccessor>();

            mockBookingRepo.Setup(repo => repo.GetBookingAsync(booking.Id, CancellationToken.None)).ReturnsAsync(() => booking);
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUserRepo.Object, mockUow.Object, mockUserAccessor.Object);

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
            var mockUserRepo = new Mock<IUserRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockUserAccessor = new Mock<IUserAccessor>();
            mockBookingRepo.Setup(repo => repo.GetPendingBookingsAsync(CancellationToken.None)).ReturnsAsync(() => bookings);
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUserRepo.Object, mockUow.Object, mockUserAccessor.Object);

            // Act
            var result = await bookingService.GetPendingBookingsAsync(CancellationToken.None);

            // Assert
            Assert.Equal(bookings, result);
            mockBookingRepo.Verify(repo => repo.GetPendingBookingsAsync(CancellationToken.None), Times.Once());
            
        }

        [Fact]
        public async Task CreateBookingAsync_CorrectData_ReturnBooking()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var user = CreateUser();
            var _event = CreateEvent();
            List<Booking> previosBookings = Enumerable.Range(0, 9).Select(_ => new Booking(Guid.NewGuid(), _event, user)).ToList();

            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockUserAccessor = new Mock<IUserAccessor>();
            var mockTransaction = new Mock<ITransaction>();
            int expectedAvailableSeats = _event.AvailableSeats - 1;
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUserRepo.Object, mockUow.Object, mockUserAccessor.Object);
            
            mockUserAccessor.Setup(accessor => accessor.GetUser()).Returns(user);
            mockUserRepo.Setup(repo => repo.GetUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockEventRepo.Setup(repo => repo.GetEventForUpdateAsync(_event.Id, CancellationToken.None)).ReturnsAsync(() => _event);
            mockBookingRepo.Setup(repo => repo.GetActiveBookingsByUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(previosBookings);
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
        public async Task CreateBookingAsync_EventCompleted_ShouldThrowCreateBookingException()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            _event.EndAt = DateTime.UtcNow.AddDays(-2);
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockUserAccessor = new Mock<IUserAccessor>();
            var mockTransaction = new Mock<ITransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUserRepo.Object, mockUow.Object, mockUserAccessor.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockEventRepo.Setup(repo => repo.GetEventForUpdateAsync(_event.Id, CancellationToken.None)).ReturnsAsync(() => _event);
            mockBookingRepo.Setup(repo => repo.AddBookingAsync(It.Is<Booking>(b => b.Id == bookingId), It.IsAny<CancellationToken>()));

            // Act & Assert
            await Assert.ThrowsAsync<CreateBookingException>(() => bookingService.CreateBookingAsync(_event.Id, CancellationToken.None));
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
            mockEventRepo.Verify(repo => repo.GetEventForUpdateAsync(_event.Id, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.AddBookingAsync(It.Is<Booking>(b => b.Id == bookingId), It.IsAny<CancellationToken>()), Times.Never());
            mockUserAccessor.Verify(repo => repo.GetUser(), Times.Never());
            mockUserRepo.Verify(repo => repo.GetUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task CreateBookingAsync_EventBegun_ShouldThrowCreateBookingException()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            _event.StartAt = DateTime.UtcNow.AddDays(-2);
            _event.EndAt = DateTime.UtcNow.AddDays(2);
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockUserAccessor = new Mock<IUserAccessor>();
            var mockTransaction = new Mock<ITransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUserRepo.Object, mockUow.Object, mockUserAccessor.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockEventRepo.Setup(repo => repo.GetEventForUpdateAsync(_event.Id, CancellationToken.None)).ReturnsAsync(() => _event);
            mockBookingRepo.Setup(repo => repo.AddBookingAsync(It.Is<Booking>(b => b.Id == bookingId), It.IsAny<CancellationToken>()));

            // Act & Assert
            await Assert.ThrowsAsync<CreateBookingException>(() => bookingService.CreateBookingAsync(_event.Id, CancellationToken.None));
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
            mockEventRepo.Verify(repo => repo.GetEventForUpdateAsync(_event.Id, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.AddBookingAsync(It.Is<Booking>(b => b.Id == bookingId), It.IsAny<CancellationToken>()), Times.Never());
            mockUserAccessor.Verify(repo => repo.GetUser(), Times.Never());
            mockUserRepo.Verify(repo => repo.GetUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task CreateBookingAsync_NoAvailableSeats_ShouldThrowNoAvalilableSeatsException()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            _event.AvailableSeats = 0;
            var user = CreateUser();

            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockUserAccessor = new Mock<IUserAccessor>();
            var mockTransaction = new Mock<ITransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUserRepo.Object, mockUow.Object, mockUserAccessor.Object);

            mockUserAccessor.Setup(accessor => accessor.GetUser()).Returns(user);
            mockUserRepo.Setup(repo => repo.GetUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockEventRepo.Setup(repo => repo.GetEventForUpdateAsync(_event.Id, CancellationToken.None)).ReturnsAsync(() => _event);
            mockBookingRepo.Setup(repo => repo.AddBookingAsync(It.Is<Booking>(b => b.Id == bookingId), It.IsAny<CancellationToken>()));

            // Act & Assert
            await Assert.ThrowsAsync<NoAvailableSeatsException>(() => bookingService.CreateBookingAsync(_event.Id, CancellationToken.None));
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
            mockUserAccessor.Verify(repo => repo.GetUser(), Times.Once());
            mockUserRepo.Verify(repo => repo.GetUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
            mockEventRepo.Verify(repo => repo.GetEventForUpdateAsync(_event.Id, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.AddBookingAsync(It.Is<Booking>(b => b.Id == bookingId), It.IsAny<CancellationToken>()), Times.Never());          
            
        }

        [Fact]
        public async Task CreateBookingAsync_BookingLimitExceeded_ShouldThrowBookingLimitExceededException()
        {
            // Arrange
            var _event = CreateEvent();
            int expectedAvailabelSeats = _event.AvailableSeats;
            var user = CreateUser();
            var bookingId = Guid.NewGuid();
            List<Booking> previosBookings = Enumerable.Range(0, 10).Select(_ => new Booking(Guid.NewGuid(), _event, user)).ToList();

            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockUserAccessor = new Mock<IUserAccessor>();
            var mockTransaction = new Mock<ITransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUserRepo.Object, mockUow.Object, mockUserAccessor.Object);

            mockUserAccessor.Setup(accessor => accessor.GetUser()).Returns(user);
            mockUserRepo.Setup(repo => repo.GetUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
            mockBookingRepo.Setup(repo => repo.GetActiveBookingsByUserAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(previosBookings);
            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockEventRepo.Setup(repo => repo.GetEventForUpdateAsync(_event.Id, CancellationToken.None)).ReturnsAsync(() => _event);
            mockBookingRepo.Setup(repo => repo.AddBookingAsync(It.Is<Booking>(b => b.Id == bookingId), It.IsAny<CancellationToken>()));

            // Act & Assert
            await Assert.ThrowsAsync<BookingLimitExceededException>(() => bookingService.CreateBookingAsync(_event.Id, CancellationToken.None));
            Assert.Equal(expectedAvailabelSeats, _event.AvailableSeats);
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
            mockUserAccessor.Verify(repo => repo.GetUser(), Times.Once());
            mockUserRepo.Verify(repo => repo.GetUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            mockEventRepo.Verify(repo => repo.GetEventForUpdateAsync(_event.Id, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.AddBookingAsync(It.Is<Booking>(b => b.Id == bookingId), It.IsAny<CancellationToken>()), Times.Never());

        }

        [Fact]
        public async Task TryProcessBooking_GoodCase_ShouldConfirmBooking()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            var user = CreateUser();
            var booking = new Booking(bookingId, _event, user);
            int expectedAvailableSeate = _event.AvailableSeats;

            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockUserAccessor = new Mock<IUserAccessor>();
            var mockTransaction = new Mock<ITransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUserRepo.Object, mockUow.Object, mockUserAccessor.Object);

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
        public async Task TryProcessBooking_BookingAlreadyProcessed_ShouldDoubleBookingProcessingException()
        {
            // Arrange
            var booking = CreateConfirmedBooking();
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockTransaction = new Mock<ITransaction>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockUserAccessor = new Mock<IUserAccessor>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUserRepo.Object, mockUow.Object, mockUserAccessor.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockBookingRepo.Setup(repo => repo.GetBookingForUpdateAsync(booking.Id, CancellationToken.None)).ReturnsAsync(booking);
            mockBookingRepo.Setup(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(), CancellationToken.None));

            // Act && Assert
            await Assert.ThrowsAsync<BookingDoubleProcessingException>(() => bookingService.TryProcessBooking(booking.Id, CancellationToken.None));
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
            mockBookingRepo.Verify(repo => repo.GetBookingForUpdateAsync(booking.Id, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task TryProcessBooking_EventCompleted_ShoultRejectBookingAndIncrementEventAwailableSeats()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            _event.StartAt = DateTime.UtcNow.AddDays(-3);
            _event.EndAt = DateTime.UtcNow.AddDays(-2);
            int expectedAvalableSeats = _event.AvailableSeats + 1;
            var user = CreateUser();
            var booking = new Booking(bookingId, _event, user);
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockUserAccessor = new Mock<IUserAccessor>();
            var mockTransaction = new Mock<ITransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUserRepo.Object, mockUow.Object, mockUserAccessor.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockBookingRepo.Setup(repo => repo.GetBookingForUpdateAsync(bookingId, CancellationToken.None)).ReturnsAsync(booking);
            mockBookingRepo.Setup(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(), CancellationToken.None));

            // Act && Assert
            await Assert.ThrowsAsync<BookingProcessException>(() => bookingService.TryProcessBooking(bookingId, CancellationToken.None));
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.CommitAsync(CancellationToken.None), Times.Once());
            mockBookingRepo.Verify(repo => repo.GetBookingForUpdateAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once());
            Assert.Equal(BookingStatus.Rejected, booking.Status);
            Assert.NotNull(booking.Event);
            Assert.Equal(booking.Event.AvailableSeats, expectedAvalableSeats);
        }

        [Fact]
        public async Task CancelBooking_UserIsBookingOwner_ShouldCancelBooking()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            var user = CreateUser();
            var booking = new Booking(bookingId, _event, user);
            int expectedAvailableSeate = _event.AvailableSeats + 1;

            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockUserAccessor = new Mock<IUserAccessor>();
            var mockTransaction = new Mock<ITransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUserRepo.Object, mockUow.Object, mockUserAccessor.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockUserAccessor.Setup(a => a.GetUser()).Returns(user);
            mockUserRepo.Setup(repo => repo.GetUserAsync(user.Name, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            mockBookingRepo.Setup(repo => repo.GetBookingForUpdateAsync(bookingId, CancellationToken.None)).ReturnsAsync(booking);
            

            // Act
            await bookingService.CancelBookingAsync(booking.Id, CancellationToken.None);

            // Assert
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.CommitAsync(CancellationToken.None), Times.Once());
            mockBookingRepo.Verify(repo => repo.GetBookingForUpdateAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.UpdateBookingAsync(It.Is<Booking>(b => b.Id == bookingId &&
                                                                                      b.Status == BookingStatus.Cancelled &&
                                                                                      b.Event != null &&
                                                                                      b.Event.AvailableSeats == expectedAvailableSeate),
                                                                                      It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task CancelBooking_UserIsAdmin_ShouldCancelBooking()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            var user = CreateUser();
            var admin = CreateUser();
            admin.Role = UserRolesEnum.Admin;
            var booking = new Booking(bookingId, _event, user);
            int expectedAvailableSeate = _event.AvailableSeats + 1;

            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockUserAccessor = new Mock<IUserAccessor>();
            var mockTransaction = new Mock<ITransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUserRepo.Object, mockUow.Object, mockUserAccessor.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockUserAccessor.Setup(a => a.GetUser()).Returns(admin);
            mockUserRepo.Setup(repo => repo.GetUserAsync(user.Name, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            mockBookingRepo.Setup(repo => repo.GetBookingForUpdateAsync(bookingId, CancellationToken.None)).ReturnsAsync(booking);

            // Act
            await bookingService.CancelBookingAsync(booking.Id, CancellationToken.None);

            // Assert
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.CommitAsync(CancellationToken.None), Times.Once());
            mockBookingRepo.Verify(repo => repo.GetBookingForUpdateAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.UpdateBookingAsync(It.Is<Booking>(b => b.Id == bookingId &&
                                                                                      b.Status == BookingStatus.Cancelled &&
                                                                                      b.Event != null &&
                                                                                      b.Event.AvailableSeats == expectedAvailableSeate),
                                                                                      It.IsAny<CancellationToken>()), Times.Once());
        }


        [Fact]
        public async Task TryCancelBooking_AnotherUser_ShouldThrowNoRightException()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var _event = CreateEvent();
            var user = CreateUser();
            var wrongUser = CreateUser();
            var booking = new Booking(bookingId, _event, user);
            int expectedAvailableSeate = _event.AvailableSeats + 1;

            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockEventRepo = new Mock<IEventRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockUserAccessor = new Mock<IUserAccessor>();
            var mockTransaction = new Mock<ITransaction>();
            var bookingService = new BookingService(mockBookingRepo.Object, mockEventRepo.Object, mockUserRepo.Object, mockUow.Object, mockUserAccessor.Object);

            mockUow.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(() => mockTransaction.Object);
            mockUserAccessor.Setup(a => a.GetUser()).Returns(wrongUser);
            mockUserRepo.Setup(repo => repo.GetUserAsync(user.Name, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            mockTransaction.Setup(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()));
            mockBookingRepo.Setup(repo => repo.GetBookingForUpdateAsync(bookingId, CancellationToken.None)).ReturnsAsync(booking);
            

            // Act & Assert
            await Assert.ThrowsAsync<NoRightForOperation>(() => bookingService.CancelBookingAsync(booking.Id, CancellationToken.None));
            mockUow.Verify(uow => uow.BeginTransactionAsync(), Times.Once());
            mockTransaction.Verify(transaction => transaction.CommitAsync(CancellationToken.None), Times.Never());
            mockBookingRepo.Verify(repo => repo.GetBookingForUpdateAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once());
            mockBookingRepo.Verify(repo => repo.UpdateBookingAsync(It.IsAny<Booking>(),It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}

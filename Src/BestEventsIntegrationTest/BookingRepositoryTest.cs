using BestEvents.Domain;
using BestEvents.Infrastructure;
using BestEvents.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace BestEventsIntegrationTest
{
    [Collection("Database collection")]
    public class BookingRepositoryTest
    {
        private readonly DatabaseFixture _fixture;

        const int TOTAL_SEATS = 10;
        readonly TimeSpan timePrecision = TimeSpan.FromMilliseconds(100);

        public BookingRepositoryTest(DatabaseFixture fixture)  
        { 
            _fixture = fixture; 
        }

        private AppDbContext CreateContext()
        {
            return _fixture.CreateContext();
        }

        private async Task InitializeDatabaseAsync()
        {
            await _fixture.ResetDatabaseAsync();
            
        }

        private static EventEntity CreateEventEntity(Guid id)
        {
            return new EventEntity
            {
                Id = id,
                Title = "Test Event",
                Description = "This is a test event.",
                StartAt = DateTime.UtcNow.AddDays(1),
                EndAt = DateTime.UtcNow.AddDays(5),
                TotalSeats = TOTAL_SEATS,
                AvailableSeats = TOTAL_SEATS,
            };
        }

        private static UserEntity CreateUserEntity(Guid userId)
        {
            return new UserEntity()
            {
                Id = userId,
                Name = "user",
                PasswordHash = "password",
                Role = UserRolesEnum.User
            };
        }

        private static BookingEntity CreateBookingEntity(EventEntity _event, UserEntity user, BookingStatus status)
        {
            return new BookingEntity()
            {
                Id = Guid.NewGuid(),
                EventId = _event.Id,
                Event = _event,
                UserId = user.Id,
                User = user,
                Status = status,
                CreatedAt = DateTime.UtcNow,
            };

        }

        private static Event CreateEvent(EventEntity eventEntity)
        {
            return Event.CreateInstanceEvent(eventEntity.Id, eventEntity.Title, eventEntity.StartAt, eventEntity.EndAt, eventEntity.Description, 
                eventEntity.TotalSeats, eventEntity.AvailableSeats);            
        }

        [Fact]
        public async Task AddBookingAsync_CorrectData_ShouldCreateBooking()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var eventEntity = CreateEventEntity(eventId);
            var userId = Guid.NewGuid();
            var userEntity = CreateUserEntity(userId);

            await context.Events.AddAsync(eventEntity, CancellationToken.None);
            await context.Users.AddAsync(userEntity, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
            var _event = CreateEvent(eventEntity);
            var bookingId = Guid.NewGuid();
            var user = User.CreateUser(userEntity.Id, userEntity.Name, userEntity.PasswordHash, userEntity.Role.ToString());
            var booking = new Booking(bookingId, _event, user);

            using var actContext = CreateContext();
            var repository = new BookingRepository(actContext, new EntityMapper());

            // Act
            await repository.AddBookingAsync(booking, CancellationToken.None);

            // Assert
            using var verifyContext = CreateContext();
            var bookingsFromDb = await verifyContext.Bookings.Where(b => b.Id == booking.Id).Include(b => b.Event).ToListAsync(CancellationToken.None);
            var bookingFromDb = bookingsFromDb.First();
            Assert.Single(bookingsFromDb);
            Assert.Equal(_event.Id, bookingFromDb.EventId);
            Assert.Equal(BookingStatus.Pending, bookingFromDb.Status);
            Assert.Equal(eventEntity.AvailableSeats, bookingFromDb.Event!.AvailableSeats);
            Assert.NotNull(bookingFromDb.Event);
            Assert.Equal(eventEntity.Title, bookingFromDb.Event.Title);
            Assert.Equal(eventEntity.Description, bookingFromDb.Event.Description);
            Assert.Equal(eventEntity.TotalSeats, bookingFromDb.Event.TotalSeats);
            Assert.Equal(eventEntity.AvailableSeats, bookingFromDb.Event.AvailableSeats);
            Assert.True(eventEntity.StartAt - bookingFromDb.Event.StartAt <= timePrecision);
            Assert.True(eventEntity.EndAt - bookingFromDb.Event.EndAt <= timePrecision);
        }

        [Fact]
        public async Task GetBookingAsync_BookingExists_ShouldReturnBookingResult()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var _event = CreateEventEntity(eventId);
            _event.AvailableSeats -= 1;          
            context.Events.Add(_event);
            var userId = Guid.NewGuid();
            var user = CreateUserEntity(userId);
            context.Users.Add(user);
            var booking = new BookingEntity
            {
                Id = Guid.NewGuid(),
                EventId = _event.Id,
                UserId = userId,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            context.Bookings.Add(booking);
            await context.SaveChangesAsync(CancellationToken.None);
            using var actContext = CreateContext();
            var repository = new BookingRepository(actContext, new EntityMapper());


            // Act
            var result = await repository.GetBookingAsync(booking.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(booking.Id, result.Id);
            Assert.Equal(booking.EventId, result.EventId);
            Assert.Equal(booking.UserId, result.UserId);
            Assert.Equal(booking.Status, result.Status);
            var dif = booking.CreatedAt - result.CreatedAt;
            Assert.True(booking.CreatedAt - result.CreatedAt <= timePrecision);
        }

        [Fact]
        public async Task GetBookingAsync_BookingDoesNotExist_ShouldThrowBookingNotFoundException()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var bookingId = Guid.NewGuid();
            var repository = new BookingRepository(context, new EntityMapper());


            // Act & Assert
            await Assert.ThrowsAsync<BookingNotFoundException>(() => repository.GetBookingAsync(bookingId, CancellationToken.None));
        }

        [Fact]
        public async Task GetBookingForUpdateAsync_BookingExists_ShouldReturnBookingResult()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var _event = CreateEventEntity(eventId);
            _event.AvailableSeats -= 1;
            context.Events.Add(_event);
            var userId = Guid.NewGuid();
            var user = CreateUserEntity(userId);
            context.Users.Add(user);
            var booking = new BookingEntity
            {
                Id = Guid.NewGuid(),
                EventId = _event.Id,
                UserId = userId,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            context.Bookings.Add(booking);
            await context.SaveChangesAsync(CancellationToken.None);
            using var actContext = CreateContext();
            var repository = new BookingRepository(actContext, new EntityMapper());


            // Act
            var result = await repository.GetBookingForUpdateAsync(booking.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(booking.Id, result.Id);
            Assert.Equal(booking.EventId, result.EventId);
            Assert.Equal(booking.UserId, result.UserId);
            Assert.Equal(booking.Status, result.Status);
            var dif = booking.CreatedAt - result.CreatedAt;
            Assert.True(booking.CreatedAt - result.CreatedAt <= timePrecision);
        }


        [Fact]
        public async Task GetBookingForUpdateAsync_BookingDoesNotExist_ShouldThrowBookingNotFoundException()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var bookingId = Guid.NewGuid();
            var repository = new BookingRepository(context, new EntityMapper());


            // Act & Assert
            await Assert.ThrowsAsync<BookingNotFoundException>(() => repository.GetBookingForUpdateAsync(bookingId, CancellationToken.None));
        }


        [Fact]
        public async Task GetPendingBookings_ThereIsSomePendingsBooking_ShouldReturnAllPendingBookings()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var _event = CreateEventEntity(eventId);
            var userId = Guid.NewGuid();    
            var user = CreateUserEntity(userId);
            int pendingQuantity = 3;
            int confirmedQuantity = 5;
            int cancelledQuantity = 7;
            int rejectedQuantity = 9;
            List<BookingEntity> pendingBookings = Enumerable.Range(0, pendingQuantity).Select(_ => CreateBookingEntity(_event, user, BookingStatus.Pending)).ToList();
            List<BookingEntity> confirmedBookings = Enumerable.Range(0, confirmedQuantity).Select(_ => CreateBookingEntity(_event, user, BookingStatus.Confirmed)).ToList();
            List<BookingEntity> cancelledBookings = Enumerable.Range(0, cancelledQuantity).Select(_ => CreateBookingEntity(_event, user, BookingStatus.Cancelled)).ToList();
            List<BookingEntity> rejectedBookings = Enumerable.Range(0, rejectedQuantity).Select(_ => CreateBookingEntity(_event, user, BookingStatus.Rejected)).ToList();

            context.Events.Add(_event);
            context.Bookings.AddRange(pendingBookings);
            context.Bookings.AddRange(confirmedBookings);
            context.Bookings.AddRange(cancelledBookings);
            context.Bookings.AddRange(rejectedBookings);

            await context.SaveChangesAsync(CancellationToken.None);

            using var actContext = CreateContext();
            var bookingRepository = new BookingRepository(actContext, new EntityMapper());

            // Act
            List<Guid> result = await bookingRepository.GetPendingBookingsAsync(CancellationToken.None);

            // Arrange
            Assert.NotNull(result);
            Assert.Equal(pendingQuantity, result.Count);
        }

        [Fact]
        public async Task GetPendingBookings_ThereIsNotPendingsBooking_ShouldReturnEmptyList()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var _event = CreateEventEntity(eventId);
            List<BookingEntity> bookings = [];
            var userId = Guid.NewGuid();
            var user = CreateUserEntity(userId);
            context.Users.Add(user);
            int confirmedQuantity = 5;
            int cancelledQuantity = 7;
            int rejectedQuantity = 9;
            List<BookingEntity> confirmedBookings = Enumerable.Range(0, confirmedQuantity).Select(_ => CreateBookingEntity(_event, user, BookingStatus.Confirmed)).ToList();
            List<BookingEntity> cancelledBookings = Enumerable.Range(0, cancelledQuantity).Select(_ => CreateBookingEntity(_event, user, BookingStatus.Cancelled)).ToList();
            List<BookingEntity> rejectedBookings = Enumerable.Range(0, rejectedQuantity).Select(_ => CreateBookingEntity(_event, user, BookingStatus.Rejected)).ToList();

            context.Events.Add(_event);
            context.Bookings.AddRange(confirmedBookings);
            context.Bookings.AddRange(cancelledBookings);
            context.Bookings.AddRange(rejectedBookings);

            await context.SaveChangesAsync(CancellationToken.None);

            using var actContext = CreateContext();
            var bookingRepository = new BookingRepository(actContext, new EntityMapper());

            // Act
            List<Guid> pendingBookings = await bookingRepository.GetPendingBookingsAsync(CancellationToken.None);

            // Arrange
            Assert.NotNull(pendingBookings);
            Assert.Empty(pendingBookings);
        }

        [Fact]
        public async Task GetUsersActiveBooking_ThereIsSomeUsersBooking_ShouldReturnAllUsersBookings()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var _event = CreateEventEntity(eventId);
            var userId = Guid.NewGuid();
            var user = CreateUserEntity(userId);
            int pendingQuantity = 3;
            int confirmedQuantity = 5;
            int cancelledQuantity = 7;
            int rejectedQuantity = 9;
            List<BookingEntity> pendingBookings = Enumerable.Range(0, pendingQuantity).Select(_ => CreateBookingEntity(_event, user, BookingStatus.Pending)).ToList();
            List<BookingEntity> confirmedBookings = Enumerable.Range(0, confirmedQuantity).Select(_ => CreateBookingEntity(_event, user, BookingStatus.Confirmed)).ToList();
            List<BookingEntity> cancelledBookings = Enumerable.Range(0, cancelledQuantity).Select(_ => CreateBookingEntity(_event, user, BookingStatus.Cancelled)).ToList();
            List<BookingEntity> rejectedBookings = Enumerable.Range(0, rejectedQuantity).Select(_ => CreateBookingEntity(_event, user, BookingStatus.Rejected)).ToList();

            context.Events.Add(_event);
            context.Bookings.AddRange(pendingBookings);
            context.Bookings.AddRange(confirmedBookings);
            context.Bookings.AddRange(cancelledBookings);
            context.Bookings.AddRange(rejectedBookings);

            await context.SaveChangesAsync(CancellationToken.None);

            using var actContext = CreateContext();
            var bookingRepository = new BookingRepository(actContext, new EntityMapper());

            // Act
            List<Booking> activeBooking = await bookingRepository.GetActiveBookingsByUserAsync(userId, CancellationToken.None);

            // Arrange
            Assert.NotNull(activeBooking);
            Assert.Equal(activeBooking.Count, pendingQuantity + confirmedQuantity);
        }


        [Fact]
        public async Task UpdateBookingAsync_EventAndBookingChanged_ShouldUpdateBookingAndEvent()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var eventId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var eventEntity = CreateEventEntity(eventId);
            var _event = CreateEvent(eventEntity);
            var userId = Guid.NewGuid();
            var user = CreateUserEntity(userId);
            _event.AvailableSeats = 5;
            DateTime bookingCreatedAt = DateTime.UtcNow.AddSeconds(-2);
            DateTime bookingProcessedAt = DateTime.UtcNow;

            var bookingEntity = new BookingEntity()
            {
                Id = bookingId,
                CreatedAt = bookingCreatedAt,
                Status = BookingStatus.Pending,
                EventId = eventId,
                UserId = userId
            };
                                                        
            using var context = CreateContext();
            context.Users.Add(user);
            context.Events.Add(eventEntity);
            context.Bookings.Add(bookingEntity);
            await context.SaveChangesAsync(CancellationToken.None);
            var booking = new Booking()
            {
                Id = bookingId,
                CreatedAt = bookingCreatedAt,
                Status = BookingStatus.Rejected,
                EventId = eventId,
                UserId = userId,
                Event = _event,
                ProcessedAt = DateTime.UtcNow
            };

            // Act
            using var actContext = CreateContext();
            var repository = new BookingRepository(actContext, new EntityMapper());
            await repository.UpdateBookingAsync(booking, CancellationToken.None);

            //Assert
            using var assertContext = CreateContext();
            var bookingFromMemory = await assertContext.Bookings.Include(b => b.Event).Include(b => b.User).FirstAsync(b => b.Id == bookingId, CancellationToken.None);
            Assert.Equal(bookingId, bookingFromMemory.Id);
            Assert.Equal(eventId, bookingFromMemory.EventId);
            Assert.Equal(userId, bookingFromMemory.UserId);
            Assert.Equal(booking.Status, bookingFromMemory.Status);
            Assert.True(bookingFromMemory.ProcessedAt > bookingFromMemory.CreatedAt);
            Assert.NotNull(bookingFromMemory.Event);
            Assert.NotNull(bookingFromMemory.User);
            Assert.Equal(_event.AvailableSeats, bookingFromMemory.Event.AvailableSeats);
        }

        [Fact]
        public async Task UpdateBookingAsync_BookingNotExists_ShouldThrowBookingNotFoundException()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var eventId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var eventEntity = CreateEventEntity(eventId);
            var _event = CreateEvent(eventEntity);
            DateTime bookingCreatedAt = DateTime.UtcNow.AddSeconds(-2);
            DateTime bookingProcessedAt = DateTime.UtcNow;

            
            var booking = new Booking()
            {
                Id = bookingId,
                CreatedAt = bookingCreatedAt,
                Status = BookingStatus.Confirmed,
                EventId = eventId,
                Event = _event,
                ProcessedAt = bookingProcessedAt,
            };

            using var actContext = CreateContext();
            var repository = new BookingRepository(actContext, new EntityMapper());

            // Act & Assert
            await Assert.ThrowsAsync<UpdateBookingException>(() => repository.UpdateBookingAsync(booking, CancellationToken.None));
        }
    }
}

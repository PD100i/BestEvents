using BestEvents.Domain;
using BestEvents.Infrastructure;
using BestEvents.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;


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
                StartAt = DateTime.UtcNow.AddDays(-5),
                EndAt = DateTime.UtcNow.AddDays(5),
                TotalSeats = TOTAL_SEATS,
                AvailableSeats = TOTAL_SEATS,
            };
        }

        private static BookingEntity CreateBookingEntity(EventEntity _event)
        {
            return new BookingEntity()
            {
                Id = Guid.NewGuid(),
                EventId = _event.Id,
                Event = _event,
                Status = BookingStatus.Pending,
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
            context.Events.Add(eventEntity);
            await context.SaveChangesAsync(CancellationToken.None);
            var _event = CreateEvent(eventEntity);
            var bookingId = Guid.NewGuid();
            var user = User.CreateUser(Guid.NewGuid(), "user", "passwordhash", "");
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
            var booking = new BookingEntity
            {
                Id = Guid.NewGuid(),
                EventId = _event.Id,
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
            var booking = new BookingEntity
            {
                Id = Guid.NewGuid(),
                EventId = _event.Id,
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
            int pendingQuentity = 10;
            List<BookingEntity> bookings = [];
            for (int i = 0; i < pendingQuentity; i++)
            {
                bookings.Add(new BookingEntity
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    Event = _event,
                    Status = BookingStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                });
            }
            for (int i = 0; i < 3; i++)
            {
                bookings.Add(new BookingEntity
                {
                    Id = Guid.NewGuid(),
                    EventId = _event.Id,
                    Event = _event,
                    Status = BookingStatus.Confirmed,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-2),
                    ProcessedAt = DateTime.UtcNow.AddMinutes(-1)
                });
            }

            context.Events.Add(_event);
            context.Bookings.AddRange(bookings);

            await context.SaveChangesAsync(CancellationToken.None);

            using var actContext = CreateContext();
            var bookingRepository = new BookingRepository(actContext, new EntityMapper());

            // Act
            List<Guid> pendingBookings = await bookingRepository.GetPendingBookingsAsync(CancellationToken.None);

            // Arrange
            Assert.NotNull(pendingBookings);
            Assert.Equal(pendingQuentity, pendingBookings.Count);
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
            for (int i = 0; i < 3; i++)
            {
                bookings.Add(new BookingEntity
                {
                    Id = Guid.NewGuid(),
                    EventId = _event.Id,
                    Event = _event,
                    Status = BookingStatus.Confirmed,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-2),
                    ProcessedAt = DateTime.UtcNow.AddMinutes(-1)
                });
            }

            context.Events.Add(_event);
            context.Bookings.AddRange(bookings);

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
        public async Task UpdateBookingAsync_EventAndBookingChanged_ShouldUpdateBookingAndEvent()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var eventId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var eventEntity = CreateEventEntity(eventId);
            var _event = CreateEvent(eventEntity);
            _event.AvailableSeats = 5;
            DateTime bookingCreatedAt = DateTime.UtcNow.AddSeconds(-2);
            DateTime bookingProcessedAt = DateTime.UtcNow;

            var bookingEntity = new BookingEntity()
            {
                Id = bookingId,
                CreatedAt = bookingCreatedAt,
                Status = BookingStatus.Pending,
                EventId = eventId,
                Event = eventEntity
            };
                                                        
            using var context = CreateContext();
            context.Events.Add(eventEntity);
            context.Bookings.Add(bookingEntity);
            await context.SaveChangesAsync(CancellationToken.None);
            var booking = new Booking()
            {
                Id = bookingId,
                CreatedAt = bookingCreatedAt,
                Status = BookingStatus.Rejected,
                EventId = eventId,
                Event = _event,
                ProcessedAt = DateTime.UtcNow
            };

            // Act
            using var actContext = CreateContext();
            var repository = new BookingRepository(actContext, new EntityMapper());
            await repository.UpdateBookingAsync(booking, CancellationToken.None);

            //Assert
            using var assertContext = CreateContext();
            var bookingFromMemory = await assertContext.Bookings.Include(b => b.Event).FirstAsync(b => b.Id == bookingId, CancellationToken.None);
            Assert.Equal(bookingId, bookingFromMemory.Id);
            Assert.Equal(eventId, bookingFromMemory.EventId);
            Assert.Equal(booking.Status, bookingFromMemory.Status);
            Assert.True(bookingFromMemory.ProcessedAt > bookingFromMemory.CreatedAt);
            Assert.NotNull(bookingFromMemory.Event);
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

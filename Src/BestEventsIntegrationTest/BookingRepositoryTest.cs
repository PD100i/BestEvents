using BestEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Threading.Tasks;
using Xunit;

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
            await _fixture.CreateContext().Database.MigrateAsync("0");
            
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

        private static Event CreateEvent(Guid id)
        {
            return new Event
            {
                Id = id,
                Title = "Test Event",
                Description = "This is a test event.",
                StartAt = DateTime.UtcNow.AddDays(1),
                EndAt = DateTime.UtcNow.AddDays(2),
                TotalSeats = TOTAL_SEATS,
                AvailableSeats = TOTAL_SEATS,
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
            var repository = new BookingRepository(context, new EntityMapper());
            var eventEntity = CreateEventEntity(eventId);
            context.Events.Add(eventEntity);
            await context.SaveChangesAsync(CancellationToken.None);
            var _event = CreateEvent(eventEntity);
            int expectedAvailableSeats = _event.AvailableSeats - 1;
            var booking = new Booking(Guid.NewGuid(), _event);
            async Task<Booking> CreateBookingStub(Event evt, CancellationToken ct) => await Task.FromResult(booking);

            // Act
            var result = await repository.AddBookingAsync(eventId, CreateBookingStub, CancellationToken.None);

            // Assert
            using var verifyContext = CreateContext();
            var bookingsFromDb = await verifyContext.Bookings.Where(b => b.Id == booking.Id).Include(b => b.Event).ToListAsync(CancellationToken.None);
            var bookingFromDb = bookingsFromDb.First();
            Assert.Single(bookingsFromDb);
            Assert.Equal(_event.Id, bookingFromDb.EventId);
            Assert.Equal(BookingStatus.Pending, bookingFromDb.Status);
            Assert.True(booking.CreatedAt - result.CreatedAt <= timePrecision);
            Assert.Equal(expectedAvailableSeats, bookingFromDb.Event!.AvailableSeats);
            Assert.NotNull(bookingFromDb.Event);
            Assert.Equal(eventEntity.Title, bookingFromDb.Event.Title);
            Assert.Equal(eventEntity.Description, bookingFromDb.Event.Description);
            Assert.Equal(eventEntity.TotalSeats, bookingFromDb.Event.TotalSeats);
            Assert.Equal(expectedAvailableSeats, bookingFromDb.Event.AvailableSeats);
            Assert.True(eventEntity.StartAt - bookingFromDb.Event.StartAt <= timePrecision);
            Assert.True(eventEntity.EndAt - bookingFromDb.Event.EndAt <= timePrecision);
        }

        [Fact]
        public async Task AddBookingAsync_Overbooking_CreateOnlyAvailableBookings()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var eventEntity = CreateEventEntity(eventId);
            context.Events.Add(eventEntity);
            await context.SaveChangesAsync(CancellationToken.None);
            var _event = CreateEvent(eventEntity);
            var booking = new Booking(Guid.NewGuid(), _event);
            async Task<Booking> CreateBookingStub(Event evt, CancellationToken ct) => await Task.FromResult(booking);

            int BOOKING_ATTEMPT = 15;

            Task<Booking>[] tasks = new Task<Booking>[BOOKING_ATTEMPT];
            for (int i = 0; i < tasks.Length; i++)
            {
                var actContext = CreateContext();
                var repository = new BookingRepository(actContext, new EntityMapper());
                tasks[i] = repository.AddBookingAsync(eventId, CreateBookingStub, CancellationToken.None);
            }

            // Act
            await Assert.ThrowsAsync<BestEvents.Exceptions.NoAvailableSeatsException>(async () => { var results = await Task.WhenAll(tasks); });

            // Assert
            using var verifyContext = CreateContext();
            var bookingsFromDb = await verifyContext.Bookings.Where(b => b.EventId == eventId).ToListAsync(CancellationToken.None);
            Assert.Equal(TOTAL_SEATS, bookingsFromDb.Count);  
        }

        [Fact]
        public async Task AddBookingAsync_EnoughSeats_CreateUnicBookingsWithSameEventId()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var eventEntity = CreateEventEntity(eventId);
            context.Events.Add(eventEntity);
            await context.SaveChangesAsync(CancellationToken.None);
            var _event = CreateEvent(eventEntity);
            var booking = new Booking(Guid.NewGuid(), _event);
            async Task<Booking> CreateBookingStub(Event evt, CancellationToken ct) => await Task.FromResult(booking);

            Task<Booking>[] tasks = new Task<Booking>[TOTAL_SEATS];
            for (int i = 0; i < tasks.Length; i++)
            {
                var actContext = CreateContext();
                var repository = new BookingRepository(actContext, new EntityMapper());
                tasks[i] = repository.AddBookingAsync(_event.Id, CreateBookingStub, CancellationToken.None);
            }        

            // Act
            var results = await Task.WhenAll(tasks);

            // Assert
            var verifyContext = CreateContext();
            var updatedEvent = await verifyContext.Events.FirstAsync(e => e.Id == _event.Id, CancellationToken.None);
            var bookings = verifyContext.Bookings.ToList();
            Assert.Equal(0, updatedEvent.AvailableSeats);
            Assert.Equal(TOTAL_SEATS, results.Select(r => r.Id).Distinct().Count());
            Assert.True(results.All(r => r.EventId == _event.Id));
            Assert.Equal(TOTAL_SEATS, bookings.Where(b => b.EventId == _event.Id).Select(b => b.Id).Distinct().Count());
        }

        [Fact]
        public async Task AddBookingAsync_EventDoesNotExist_ShouldThrowEventNotFoundException()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var repository = new BookingRepository(context, new EntityMapper());
            await context.SaveChangesAsync(CancellationToken.None);
            var _event = CreateEvent(eventId);
            var booking = new Booking(Guid.NewGuid(), _event);
            async Task<Booking> CreateBookingStub(Event evt, CancellationToken ct) => await Task.FromResult(booking);

            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.EventNotFoundException>(() => repository.AddBookingAsync(Guid.NewGuid(), CreateBookingStub, CancellationToken.None));
        }

        [Fact]
        public async Task AddBookingAsync_NoAwailableSeats_ShouldThrowNoAvailableSeatsException()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var repository = new BookingRepository(context, new EntityMapper());
            var eventEntity = CreateEventEntity(eventId);
            eventEntity.AvailableSeats = 0;
            context.Events.Add(eventEntity);
            await context.SaveChangesAsync(CancellationToken.None);
            var _event = CreateEvent(eventEntity);
            var booking = new Booking(Guid.NewGuid(), _event);
            async Task<Booking> CreateBookingStub(Event evt, CancellationToken ct) => await Task.FromResult(booking);

            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.NoAvailableSeatsException>(() => repository.AddBookingAsync(eventId, CreateBookingStub, CancellationToken.None));        
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
            await Assert.ThrowsAsync<BestEvents.Exceptions.BookingNotFoundException>(() => repository.GetBookingAsync(bookingId, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateBookingAsync_CorrectData_ShouldUpdateBooking()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var eventId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var eventEntity = CreateEventEntity(eventId);
            var _event = CreateEvent(eventEntity);
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

            var updatedBooking = new Booking()
            {
                Id = bookingId,
                CreatedAt = bookingCreatedAt,
                Status = BookingStatus.Confirmed,
                EventId = eventId,
                Event = _event,
                ProcessedAt = bookingProcessedAt,
            };

            using var context = CreateContext();
            context.Events.Add(eventEntity);
            context.Bookings.Add(bookingEntity);
            await context.SaveChangesAsync(CancellationToken.None);

            // Act
            using var actContext = CreateContext();
            var repository = new BookingRepository(actContext, new EntityMapper());
            await repository.UpdateBookingAsync(updatedBooking, CancellationToken.None);

            //Assert
            using var assertContext = CreateContext();
            var bookingFromMemory = await assertContext.Bookings.FirstAsync(b => b.Id == bookingId, CancellationToken.None);
            Assert.Equal(updatedBooking.Id, bookingFromMemory.Id);
            Assert.Equal(updatedBooking.EventId, bookingFromMemory.EventId);
            Assert.Equal(eventEntity, bookingFromMemory.Event);
            Assert.Equal(updatedBooking.Status, bookingFromMemory.Status);
            Assert.True(updatedBooking.CreatedAt - bookingFromMemory.CreatedAt < timePrecision);
            Assert.True(updatedBooking.ProcessedAt - bookingFromMemory.ProcessedAt < timePrecision);
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

            
            var updatedBooking = new Booking()
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
            await Assert.ThrowsAsync<BestEvents.Exceptions.BookingNotFoundException>(() => repository.UpdateBookingAsync(updatedBooking, CancellationToken.None));
        }
    }
}

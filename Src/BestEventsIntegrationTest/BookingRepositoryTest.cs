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
            var bookingId = Guid.NewGuid();
            var booking = new Booking(bookingId, _event);
            Booking CreateBookingStub(Guid id, Event _event) => booking;

            // Act
            var result = await repository.AddBookingAsync(bookingId, eventId, CreateBookingStub, CancellationToken.None);

            // Assert
            using var verifyContext = CreateContext();
            var bookingsFromDb = await verifyContext.Bookings.Where(b => b.Id == booking.Id).Include(b => b.Event).ToListAsync(CancellationToken.None);
            var bookingFromDb = bookingsFromDb.First();
            Assert.Single(bookingsFromDb);
            Assert.Equal(_event.Id, bookingFromDb.EventId);
            Assert.Equal(BookingStatus.Pending, bookingFromDb.Status);
            Assert.True(booking.CreatedAt - result.CreatedAt <= timePrecision);
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
        public async Task AddBookingAsync_EventDoesNotExist_ShouldThrowEventNotFoundException()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var bookingId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var repository = new BookingRepository(context, new EntityMapper());
            await context.SaveChangesAsync(CancellationToken.None);
            var _event = CreateEvent(eventId);
            var booking = new Booking(Guid.NewGuid(), _event);
            Booking CreateBookingStub(Guid bookingId, Event _event) => booking;

            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.EventNotFoundException>(() => repository.AddBookingAsync(bookingId, Guid.NewGuid(), CreateBookingStub, CancellationToken.None));
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
        public async Task UpdateBookingAsync_EventAndBookingChanged_ShouldUpdateBookingAndEvent()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var eventId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var eventEntity = CreateEventEntity(eventId);
            int expectedAvailableSeats = eventEntity.AvailableSeats + 1;
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

            Action<Booking> updateAction = b =>
            {
                b.ProcessedAt = DateTime.UtcNow;
                b.Status = BookingStatus.Rejected;
                b.Event!.AvailableSeats = expectedAvailableSeats;
            };
                                                        
            using var context = CreateContext();
            context.Events.Add(eventEntity);
            context.Bookings.Add(bookingEntity);
            await context.SaveChangesAsync(CancellationToken.None);

            // Act
            using var actContext = CreateContext();
            var repository = new BookingRepository(actContext, new EntityMapper());
            await repository.UpdateBookingAsync(bookingId, updateAction, CancellationToken.None);

            //Assert
            using var assertContext = CreateContext();
            var bookingFromMemory = await assertContext.Bookings.Include(b => b.Event).FirstAsync(b => b.Id == bookingId, CancellationToken.None);
            Assert.Equal(bookingId, bookingFromMemory.Id);
            Assert.Equal(eventId, bookingFromMemory.EventId);
            Assert.Equal(BookingStatus.Rejected, bookingFromMemory.Status);
            Assert.True(bookingFromMemory.ProcessedAt > bookingFromMemory.CreatedAt);
            Assert.NotNull(bookingFromMemory.Event);
            Assert.Equal(expectedAvailableSeats, bookingFromMemory.Event.AvailableSeats);
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
            void updateAction(Booking booking) => booking = updatedBooking;

            using var actContext = CreateContext();
            var repository = new BookingRepository(actContext, new EntityMapper());

            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.BookingNotFoundException>(() => repository.UpdateBookingAsync(bookingId, updateAction, CancellationToken.None));
        }
    }
}

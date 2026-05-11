using BestEvents;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Threading.Tasks;
using Xunit;

namespace BestEventsIntegrationTest
{
    [Collection("Database collection")]
    public class BookingServiceTest
    {
        private readonly DatabaseFixture _fixture;

        const int TOTAL_SEATS = 10;
        readonly TimeSpan timePrecision = TimeSpan.FromMilliseconds(100);

        public BookingServiceTest(DatabaseFixture fixture)  
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

        private static EventEntity CreateEvent()
        {
            return new EventEntity
            {
                Id = Guid.NewGuid(),
                Title = "Test Event",
                Description = "This is a test event.",
                StartAt = DateTime.UtcNow.AddDays(1),
                EndAt = DateTime.UtcNow.AddDays(2),
                TotalSeats = TOTAL_SEATS,
                AvailableSeats = TOTAL_SEATS,
            };
        }

        [Fact]
        public async Task CreateBookingAsync_CorrectData_ShouldCreateBooking()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var bookingService = new BookingService(context, new EntityMapper());
            var _event = CreateEvent();
            context.Events.Add(_event);
            await context.SaveChangesAsync(CancellationToken.None);
            int expectedAvailableSeats = _event.AvailableSeats - 1;

            // Act
            var booking = await bookingService.CreateBookingAsync(_event.Id, CancellationToken.None);

            // Assert
            using var verifyContext = CreateContext();
            var bookingInDb = await verifyContext.Bookings.Where(b => b.Id == booking.Id).Include(b => b.Event).ToListAsync(CancellationToken.None);
            Assert.Single(bookingInDb);
            Assert.Equal(_event.Id, bookingInDb[0].EventId);
            Assert.Equal(BookingStatus.Pending, bookingInDb[0].Status);
            Assert.NotNull(bookingInDb[0].Event);
            Assert.Equal(expectedAvailableSeats, bookingInDb[0].Event!.AvailableSeats);

        }

        

        [Fact]
        public async Task CreateBookingAsync_Overbooking_CreateOnlyAvailableBookings()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var _event = CreateEvent();
            context.Events.Add(_event);
            await context.SaveChangesAsync(CancellationToken.None);
            
            int BOOKING_ATTEMPT = 15;

            Task<Booking>[] tasks = new Task<Booking>[BOOKING_ATTEMPT];
            for (int i = 0; i < tasks.Length; i++)
            {
                var actContext = CreateContext();
                var bookingService = new BookingService(actContext, new EntityMapper());
                tasks[i] = bookingService.CreateBookingAsync(_event.Id, CancellationToken.None);
            }

            // Act
            await Assert.ThrowsAsync<BestEvents.Exceptions.NoAvailableSeatsException>(async () => { var results = await Task.WhenAll(tasks); });

            // Assert
            using var verifyContext = CreateContext();
            var bookingsInDb = await verifyContext.Bookings.Where(b => b.EventId == _event.Id).ToListAsync(CancellationToken.None);
            Assert.Equal(TOTAL_SEATS, bookingsInDb.Count);  
        }

        [Fact]
        public async Task CreateBookingAsync_EnoughSeats_CreateUnicBookingsWithSameEventId()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var _event = CreateEvent();
            context.Events.Add(_event);
            await context.SaveChangesAsync(CancellationToken.None);

            Task<Booking>[] tasks = new Task<Booking>[TOTAL_SEATS];
            for (int i = 0; i < tasks.Length; i++)
            {
                var actContext = CreateContext();
                var bookingService = new BookingService(actContext, new EntityMapper());
                tasks[i] = bookingService.CreateBookingAsync(_event.Id, CancellationToken.None);
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
        public async Task CreateBookingAsync_EventDoesNotExist_ShouldThrowEventNotFoundException()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var bookingService = new BookingService(context, new EntityMapper());
            var _event = CreateEvent();

            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.EventNotFoundException>(() => bookingService.CreateBookingAsync(Guid.NewGuid(), CancellationToken.None));
        }

        [Fact]
        public async Task CreateBookingAsync_NoAwailableSeats_ShouldThrowNoAvailableSeatsException()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var _event = CreateEvent();
            _event.AvailableSeats = 0;
            context.Events.Add(_event);
            context.SaveChanges();
            using var actContext = CreateContext();

            var bookingService = new BookingService(actContext, new EntityMapper());

            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.NoAvailableSeatsException>(() => bookingService.CreateBookingAsync(_event.Id, CancellationToken.None));
        
        }

        [Fact]
        public async Task GetBookingByIdAsync_BookingExists_ShouldReturnBookingResult()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var _event = CreateEvent();
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
            var bookingService = new BookingService(actContext, new EntityMapper());


            // Act
            var result = await bookingService.GetBookingByIdAsync(booking.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(booking.Id, result.Id);
            Assert.Equal(booking.EventId, result.EventId);
            Assert.Equal(booking.Status, result.Status);
            var dif = booking.CreatedAt - result.CreatedAt;
            Assert.True(booking.CreatedAt - result.CreatedAt <= timePrecision);
        }

        [Fact]
        public async Task GetBookingByIdAsync_BookingDoesNotExist_ShouldThrowDataNotFoundException()
        {
            // Arrange
            await InitializeDatabaseAsync();
            using var context = CreateContext();
            var bookingId = Guid.NewGuid();
            var bookingService = new BookingService(context, new EntityMapper());


            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.BookingNotFoundException>(() => bookingService.GetBookingByIdAsync(bookingId, CancellationToken.None));
        }
    }
}

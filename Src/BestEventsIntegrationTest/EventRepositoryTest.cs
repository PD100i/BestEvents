using BestEvents.Application;
using BestEvents.Domain;
using BestEvents.Domain.Exceptions;
using BestEvents.Infrastructure;
using BestEvents.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace BestEventsIntegrationTest
{
    [Collection("Database collection")]
    public class EventRepositoryTest
    {
        private readonly DatabaseFixture _fixture;

        public EventRepositoryTest(DatabaseFixture fixture)
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

        private EventRepository CreateEventRepository(AppDbContext dbContext)
        {
            return new EventRepository(dbContext, new EntityMapper(), new EventFilters(), new Pagination<EventEntity>());
        }

        private async Task AddCollection(AppDbContext context)
        {
            await context.Events.AddRangeAsync(EventCollection.GetCollection());
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task AddEvent_ShouldWriteToDatabaseAndReturnCreatedEvent()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var context = CreateContext();
            EventRepository eventService = CreateEventRepository(context);
            var _event = new Event()
            {
                Id = Guid.Parse("2f3bf53d-ee2d-4973-9aca-93f767e7d40f"),
                Title = "Весенняя ярмарка ремёсел",
                StartAt = new DateTime(2025, 04, 15),
                EndAt = new DateTime(2025, 04, 20),
                TotalSeats = 1000,
                AvailableSeats = 100
            };


            // Act
            Event result = await eventService.AddEventAsync(_event, CancellationToken.None);

            // Assert
            var assertContext = CreateContext();
            EventEntity? eventEntity = await assertContext.Events.FirstOrDefaultAsync(e => e.Id == _event.Id, CancellationToken.None);
            Assert.Equal(result, _event);
            Assert.NotNull(eventEntity);
            Assert.Equal(result.Id, eventEntity.Id);
            Assert.Equal(result.Title, eventEntity.Title);
            Assert.Equal(result.StartAt, eventEntity.StartAt);
            Assert.Equal(result.EndAt, eventEntity.EndAt);
            Assert.Equal(result.TotalSeats, eventEntity.TotalSeats);
            Assert.Equal(result.AvailableSeats, eventEntity.AvailableSeats);

        }

        [Fact]
        public async Task DeleteEvent_CallWithExistedId_ShouldRemoveFromDb()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var context = CreateContext();

            var user = new UserEntity()
            {
                Id = Guid.NewGuid(),
                Name = "user",
                PasswordHash = "password",
                Role = UserRolesEnum.User
            };
            context.Users.Add(user);
            await AddCollection(context);
            List<BookingEntity> bookings = new List<BookingEntity> {
                new BookingEntity { Id = Guid.NewGuid(), EventId = EventCollection.GetEventEntity(2).Id, UserId = user.Id },
                new BookingEntity { Id = Guid.NewGuid(), EventId = EventCollection.GetEventEntity(2).Id, UserId = user.Id }
            };
            await context.Bookings.AddRangeAsync(bookings, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);

            var id = EventCollection.GetEventEntity(2).Id;

            var actContext = CreateContext();
            var eventRepository = CreateEventRepository(actContext);

            // Act 
            await eventRepository.DeleteEventAsync(id, CancellationToken.None);

            // Assert
            var assertContext = CreateContext();
            var eventEntity = await assertContext.Events.FirstOrDefaultAsync(e => e.Id == id, CancellationToken.None);
            var _bookings = await assertContext.Bookings.Where(e => e.EventId == id).ToListAsync(CancellationToken.None);
            Assert.Null(eventEntity);
            Assert.Empty(_bookings);
        }


        [Fact]
        public async Task DeleteEvent_CallWithNotExistedId_ShouldThrowEventNotFoundException()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var context = CreateContext();
            await AddCollection(context);
            var id = Guid.NewGuid();

            var assertContext = CreateContext();
            var eventRepository = CreateEventRepository(assertContext);

            // Act & Assert
            await Assert.ThrowsAsync<EventNotFoundException>(() => eventRepository.DeleteEventAsync(id, CancellationToken.None));
        }

        [Fact]
        public async Task GetEvent_CallWithExistedId_ShouldReturnEvent()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var context = CreateContext();
            await AddCollection(context);

            var _event = EventCollection.GetEventEntity(2);
            var id = _event.Id;

            var actContext = CreateContext();
            var eventRepository = CreateEventRepository(actContext);

            // Act
            Event result = await eventRepository.GetEventAsync(id, CancellationToken.None);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Id, _event.Id);
            Assert.Equal(result.Title, _event.Title);
            Assert.Equal(result.StartAt, _event.StartAt);
            Assert.Equal(result.EndAt, _event.EndAt);
            Assert.Equal(result.Description, _event.Description);
        }


        [Fact]
        public async Task GetEvent_NotExistedId_NotFoundExceotion()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var context = CreateContext();
            await AddCollection(context);
            var id = Guid.NewGuid();

            var actContext = CreateContext();
            var eventRepository = CreateEventRepository(actContext);

            // Act & Assert
            await Assert.ThrowsAsync<EventNotFoundException>(() => eventRepository.GetEventAsync(id, CancellationToken.None));
        }

        [Fact]
        public async Task GetEventForUpdate_CallWithExistedId_ShouldReturnEvent()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var context = CreateContext();
            await AddCollection(context);

            var _event = EventCollection.GetEventEntity(2);
            var id = _event.Id;

            var actContext = CreateContext();
            var eventRepository = CreateEventRepository(actContext);

            // Act
            Event result = await eventRepository.GetEventForUpdateAsync(id, CancellationToken.None);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Id, _event.Id);
            Assert.Equal(result.Title, _event.Title);
            Assert.Equal(result.StartAt, _event.StartAt);
            Assert.Equal(result.EndAt, _event.EndAt);
            Assert.Equal(result.Description, _event.Description);
        }


        [Fact]
        public async Task GetEventForUpdate_NotExistedId_NotFoundExceotion()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var context = CreateContext();
            await AddCollection(context);
            var id = Guid.NewGuid();

            var actContext = CreateContext();
            var eventRepository = CreateEventRepository(actContext);

            // Act & Assert
            await Assert.ThrowsAsync<EventNotFoundException>(() => eventRepository.GetEventForUpdateAsync(id, CancellationToken.None));
        }

        [Fact]
        public async Task ReplaceEvent_ShouldReplaceInDbAndReturnEvent()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var context = CreateContext();
            await AddCollection(context);
            var entity = EventCollection.GetEventEntity(2);
            string newTitle = "ReplacedTitle";

            var replacedEntity = new EventEntity()
            {
                Id = entity.Id,
                Title = newTitle,
                StartAt = entity.StartAt,
                EndAt = entity.EndAt,
                TotalSeats = entity.TotalSeats,
                AvailableSeats = entity.AvailableSeats
            };

            var replacedEvent = new Event()
            {
                Id = replacedEntity.Id,
                Title = replacedEntity.Title,
                StartAt = replacedEntity.StartAt,
                EndAt = replacedEntity.EndAt,
                TotalSeats = replacedEntity.TotalSeats,
                AvailableSeats = replacedEntity.AvailableSeats
            };

            var actContext = CreateContext();
            var eventRepository = CreateEventRepository(actContext);

            // Act
            await eventRepository.ReplaceEventAsync(replacedEvent, CancellationToken.None);

            // Assert
            var assertContext = CreateContext();

            EventEntity? result = await assertContext.Events.FirstOrDefaultAsync(e => e.Id == entity.Id, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(result, replacedEntity);

        }

        [Fact]
        public async Task ReplaceEvent_NotFoundEvent_ShouldThrowEventNotFoundException()
        {
            // Arrange
            await InitializeDatabaseAsync();
            var context = CreateContext();
            await AddCollection(context);
            var _event = new Event()
            {
                Id = Guid.Parse("2f3bf53d-ee2d-4973-9aca-93f767e7d401"),
                Title = "Весенняя ярмарка ремёсел",
                StartAt = new DateTime(2025, 04, 15),
                EndAt = new DateTime(2025, 04, 20),
                TotalSeats = 1000,
                AvailableSeats = 100
            };

            var assertContext = CreateContext();
            var eventRepository = CreateEventRepository(assertContext);

            // Act & Assert
            await Assert.ThrowsAsync<UpdateEventException>(() => eventRepository.ReplaceEventAsync(_event, CancellationToken.None));
        }

        

        public static IEnumerable<object?[]> GetEvents_CorrectArguments()
        {

            List<Event> events = EventCollection.GetEventCollection();
            return new List<object?[]>
            {
                new object?[] { null, null, null, 1, 10,
                    new PaginatedResult<Event>() { TotalResultsNumber = events.Count, CurrentPage = 1, ResultsNumberOnPage = 10, ResultsOnPage = events.GetRange(0, 10)} },
                new object?[] { "Event", null, null, 1, 10,
                    new PaginatedResult<Event>() { TotalResultsNumber = 2, CurrentPage = 1, ResultsNumberOnPage = 2, ResultsOnPage = events.GetRange(10, 2)} },
                new object?[] { null, DateTime.SpecifyKind(new DateTime(2025, 06, 10), DateTimeKind.Utc), DateTime.SpecifyKind(new DateTime(2026, 12, 20), DateTimeKind.Utc), 3, 4,
                    new PaginatedResult<Event>() { TotalResultsNumber = 9, CurrentPage = 3, ResultsNumberOnPage = 1, ResultsOnPage = events.GetRange(9, 1)} },
                new object?[] { "фести", DateTime.SpecifyKind(new DateTime(2025, 06, 10), DateTimeKind.Utc), DateTime.SpecifyKind(new DateTime(2026, 12, 20), DateTimeKind.Utc), 1, 10,
                    new PaginatedResult<Event>() { TotalResultsNumber = 2, CurrentPage = 1, ResultsNumberOnPage = 2, ResultsOnPage = [ events[1], events[5] ] } }
            };
        }

        [Theory]
        [MemberData(nameof(GetEvents_CorrectArguments))]
        public async Task GetEvents_CallWithCorrectArguments_ShouldReturnPaginatedResult(string? title, DateTime? from, DateTime? to, int page, int size, PaginatedResult<Event> expectedResult)
        {
            // Arrange
            await InitializeDatabaseAsync();
            var context = CreateContext();
            await AddCollection(context);

            var actContext = CreateContext();
            var eventRepository = CreateEventRepository(actContext);

            // Act
            var result = await eventRepository.GetEventsAsync(title, from, to, page, size, CancellationToken.None);
            // Assert
            Assert.Equal(result, expectedResult);
        }


        public static IEnumerable<object?[]> Get_GetEvents_WrongArguments()
        {

            return new List<object?[]>
            {
                new object?[] { null, null, null, 0, 10 },
                new object?[] { null, null, null, 1, 0 }
            };
        }

        [Theory]
        [MemberData(nameof(Get_GetEvents_WrongArguments))]
        public async Task GetEvents_CallWithWrongArguments_ShouldThrowException(string? title, DateTime? from, DateTime? to, int page, int size)
        {
             // Arrange
            await InitializeDatabaseAsync();
            var context = CreateContext();
            await AddCollection(context);

            var actContext = CreateContext();
            var eventRepository = CreateEventRepository(actContext);

            // Act & Assert 
            await Assert.ThrowsAsync<EventWrongParameterException>(() => eventRepository.GetEventsAsync(title, from, to, page, size, CancellationToken.None));
        }
    }
}

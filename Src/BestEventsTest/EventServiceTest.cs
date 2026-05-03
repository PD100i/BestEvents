using BestEvents;
using BestEvents.Exceptions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace BestEventsTest
{
    public class EventServiceFixture : IDisposable
    {
        public AppDbContext Context { get; }
        public EventService EventService { get; }

        public EventServiceFixture()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: "TestDatabase" + Guid.NewGuid().ToString())
                    .Options;

            Context = new AppDbContext(options);
            EventService = new EventService(Context, new EntityMapper(), new EventFilters(), new Pagination<EventEntity>());
        }

        public async Task AddCollection()
        {
            await Context.Events.AddRangeAsync(EventCollection.GetCollection());
            await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }

    public class EventServiceTest()
    {

        [Fact]
        public async Task CreateEvent_ShouldWriteToDatabaseAndReturnCreatedEvent()
        {
            // Arrange
            using var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
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
            Event result = await eventService.CreateEventAsync(_event, CancellationToken.None);
            EventEntity? eventEntity = await fixture.Context.Events.FirstOrDefaultAsync(e => e.Id == _event.Id, CancellationToken.None);

            // Assert
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
            using var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
            await fixture.Context.Database.EnsureCreatedAsync(CancellationToken.None);
            await fixture.AddCollection();
            List<BookingEntity> bookings = new List<BookingEntity> {
                new BookingEntity { Id = Guid.NewGuid(), EventId = EventCollection.GetEventEntity(2).Id },
                new BookingEntity { Id = Guid.NewGuid(), EventId = EventCollection.GetEventEntity(2).Id }
            };
            await fixture.Context.Bookings.AddRangeAsync(bookings, CancellationToken.None);
            await fixture.Context.SaveChangesAsync(CancellationToken.None);
            var id = EventCollection.GetEventEntity(2).Id;

            // Act 
            await eventService.DeleteEventAsync(id, CancellationToken.None);

            // Assert
            var eventEntity = await fixture.Context.Events.FirstOrDefaultAsync(e => e.Id == id, CancellationToken.None);
            var _bookings = await fixture.Context.Bookings.Where(e => e.EventId == id).ToListAsync(CancellationToken.None);
            Assert.Null(eventEntity);
            Assert.All(_bookings, booking => Assert.Null(booking.Event));
        }


        [Fact]
        public async Task DeleteEvent_CallWithNotExistedId_ShouldThrowEventNotFoundException()
        {
            // Arrange
            using var fixture = new EventServiceFixture();
            await fixture.Context.Database.EnsureCreatedAsync(CancellationToken.None);
            EventService eventService = fixture.EventService;
            await fixture.AddCollection();
            var id = Guid.Parse("349b6818-0d33-43ed-94e4-84824b09eee1");

            // Act & Assert
            await Assert.ThrowsAsync<EventNotFoundException>(() => eventService.DeleteEventAsync(id, CancellationToken.None));
        }

        [Fact]
        public async Task GetEvent_CallWithExistedId_ShouldReturnEvent()
        {
            // Arrange
            using var fixture = new EventServiceFixture();
            await fixture.Context.Database.EnsureCreatedAsync(CancellationToken.None);
            EventService eventService = fixture.EventService;
            await fixture.AddCollection();
            var _event = EventCollection.GetEventEntity(2);
            var id = _event.Id;

            // Act
            Event result = await eventService.GetEventAsync(id, CancellationToken.None);
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
            using var fixture = new EventServiceFixture();
            await fixture.Context.Database.EnsureCreatedAsync(CancellationToken.None);
            EventService eventService = fixture.EventService;
            await fixture.AddCollection();
            var id = Guid.Parse("349b6818-0d33-43ed-94e4-84824b09eee1");

            // Act & Assert
            await Assert.ThrowsAsync<EventNotFoundException>(() => eventService.GetEventAsync(id, CancellationToken.None));
        }

        [Fact]
        public async Task ReplaceEvent_ShouldReplaceInDbAndReturnEvent()
        {
            // Arrange
            using var fixture = new EventServiceFixture();
            await fixture.Context.Database.EnsureCreatedAsync(CancellationToken.None);
            EventService eventService = fixture.EventService;
            await fixture.AddCollection();
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

            // Act
            await eventService.ReplaceEventAsync(replacedEvent.Id, replacedEvent, CancellationToken.None);

            // Assert
            EventEntity? result = await fixture.Context.Events.FirstOrDefaultAsync(e => e.Id == entity.Id, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(result, replacedEntity);

        }

        [Fact]
        public async Task ReplaceEvent_NotFoundEvent_ShouldThrowEventNotFoundException()
        {
            // Arrange
            using var fixture = new EventServiceFixture();
            await fixture.Context.Database.EnsureCreatedAsync(CancellationToken.None);
            EventService eventService = fixture.EventService;
            await fixture.AddCollection();
            var _event = new Event()
            {
                Id = Guid.Parse("2f3bf53d-ee2d-4973-9aca-93f767e7d401"),
                Title = "Весенняя ярмарка ремёсел",
                StartAt = new DateTime(2025, 04, 15),
                EndAt = new DateTime(2025, 04, 20),
                TotalSeats = 1000,
                AvailableSeats = 100
            };


            // Act & Assert
            await Assert.ThrowsAsync<EventNotFoundException>(() => eventService.ReplaceEventAsync(_event.Id, _event, CancellationToken.None));
        }

        [Fact]
        public async Task ReplaceEvent_MismatchId_ShouldThrowEventWrongParameterException()
        {
            // Arrange
            using var fixture = new EventServiceFixture();
            await fixture.Context.Database.EnsureCreatedAsync(CancellationToken.None);
            EventService eventService = fixture.EventService;
            await fixture.AddCollection();
            var wrongId = Guid.Parse("349b6818-0d33-43ed-94e4-84824b09eee1");
            var _event = new Event()
            {
                Id = Guid.Parse("2f3bf53d-ee2d-4973-9aca-93f767e7d40f"),
                Title = "Весенняя ярмарка ремёсел",
                StartAt = new DateTime(2025, 04, 15),
                EndAt = new DateTime(2025, 04, 20),
                TotalSeats = 1000,
                AvailableSeats = 100
            };
            // Act & Assert
            await Assert.ThrowsAsync<EventWrongParameterException>(() => eventService.ReplaceEventAsync(wrongId, _event, CancellationToken.None));
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
                //new object?[] { null, new DateTime(2025, 06, 10), new DateTime(2026, 12, 20), 3, 4,
                //    new PaginatedResult<Event>() { TotalResultsNumber = 9, CurrentPage = 3, ResultsNumberOnPage = 1, ResultsOnPage = events.GetRange(9, 1)} },
                //new object?[] { "фести", new DateTime(2025, 06, 10), new DateTime(2026, 12, 20), 1, 10, 
                //    new PaginatedResult<Event>() { TotalResultsNumber = 2, CurrentPage = 1, ResultsNumberOnPage = 2, ResultsOnPage = [ events[1], events[5] ] } }
            };
        }

        [Theory]
        [MemberData(nameof(GetEvents_CorrectArguments))]
        public async Task GetEvents_CallWithCorrectArguments_ShouldReturnPaginatedResult(string? title, DateTime? from, DateTime? to, int page, int size, PaginatedResult<Event> expectedResult)
        {
            // Arrange
            using var fixture = new EventServiceFixture();
            await fixture.Context.Database.EnsureCreatedAsync(CancellationToken.None);
            EventService eventService = fixture.EventService;
            await fixture.AddCollection();

            // Act && Assert
            var result = await eventService.GetEventsAsync(title, from, to, page, size, CancellationToken.None);
            Assert.Equal(result, expectedResult);
        }


        public static IEnumerable<object?[]> Get_GetEvents_WrongArguments()
        {

            return new List<object?[]>
            {
                new object?[] { null, new DateTime(2025, 06, 10), new DateTime(2024, 06, 10), 1, 10 },
                new object?[] { null, null, null, 0, 10 },
                new object?[] { null, null, null, 1, 0 }
            };
        }

        [Theory]
        [MemberData(nameof(Get_GetEvents_WrongArguments))]
        public async Task GetEvents_CallWithWrongArguments_ShouldThrowException(string? title, DateTime? from, DateTime? to, int page, int size)
        {
            // Arrange
            using var fixture = new EventServiceFixture();
            await fixture.Context.Database.EnsureCreatedAsync(CancellationToken.None);
            EventService eventService = fixture.EventService;
            await fixture.AddCollection();

            // Act & Assert 
            await Assert.ThrowsAsync<EventWrongParameterException>(() => eventService.GetEventsAsync(title, from, to, page, size, CancellationToken.None));
        }
    }
}

using BestEvents;
using BestEvents.Exceptions;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BestEventsTest
{
    public class EventServiceFixture
    {
        public Mock<IEventRepository> MockEventRepository { get; set; }

        public EventService EventService { get; set; }

        public EventServiceFixture()
        {
            MockEventRepository = new Mock<IEventRepository>();
            EventService = new EventService(MockEventRepository.Object, new EventFilters(), new Pagination<Event>());
        }
    }

    public class EventServiceTest()
    {
        

        public static IEnumerable<object?[]> GetEventCorrectArguments()
        {
            Event _event = EventCollection.GetEvent(0);
            return new List<object?[]>
            {
                new object?[] { _event.Id, _event.Title, _event.StartAt, _event.EndAt, _event.TotalSeats, "Some description", "Some description" },
                new object?[] { _event.Id, _event.Title, _event.StartAt, _event.EndAt, _event.TotalSeats, "", "" },
                new object?[] { _event.Id, _event.Title, _event.StartAt, _event.EndAt, _event.TotalSeats, null, "" }
            };
        }

        [Theory]
        [MemberData(nameof(GetEventCorrectArguments))]
        public async Task CreateEvent_CallWithCorrectArguments_CallRepoMethodReturnEvent(Guid id, string title, DateTime? startAt, DateTime? endAt, int? totalSeats, string? description, string expectedDescription)
        {
            // Arrange
            var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
            Mock<IEventRepository> mockRepository = fixture.MockEventRepository;
            var _event = Event.CreateEvent(id, title, startAt, endAt, description, totalSeats, totalSeats);


            mockRepository.Setup(mock => mock.AddEventAsync(It.Is<Event>(e => e.Title == title &&
                                                                              e.StartAt == startAt &&
                                                                              e.EndAt == endAt &&
                                                                              e.TotalSeats == totalSeats &&
                                                                              e.AvailableSeats == totalSeats &&
                                                                              e.Description == expectedDescription)))
                                                                              .ReturnsAsync(_event);
            // Act
            EventInfoDto result = await eventService.CreateEventAsync(new CreateEventDto(title, startAt, endAt, description, totalSeats), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Id, id.ToString());
            Assert.Equal(result.Title, title);
            Assert.Equal(result.StartAt, startAt);
            Assert.Equal(result.EndAt, endAt);
            Assert.Equal(result.Description, expectedDescription);
            Assert.Equal(result.TotalSeats, totalSeats!.Value);
            Assert.Equal(result.AvailableSeats, totalSeats!.Value);
        }


        public static IEnumerable <object?[]> Get_CreateEvent_WrongArguments()
        {
            Event _event = EventCollection.GetEvent(0);
            return new List<object?[]>
            {
                 new object?[] { "", _event.StartAt, _event.EndAt, _event.TotalSeats },
                 new object?[] { _event.Title, null, _event.EndAt, _event.TotalSeats },
                 new object?[] { _event.Title, _event.StartAt, null, _event.TotalSeats },
                 new object?[] { _event.Title, _event.StartAt, _event.EndAt, null },
                 new object?[] { _event.Title, _event.StartAt, _event.EndAt, 0 },
                 new object?[] { "", null, null, null }
            };
    
        }

        [Theory]
        [MemberData(nameof(Get_CreateEvent_WrongArguments))]
        public async Task CreateEvent_CallWithWrongArguments_Exception(string title, DateTime? startAt, DateTime? endAt, int totalSeats)
        {
            //Arrange
            var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
            Mock<IEventRepository> mockRepository = fixture.MockEventRepository;
            // Act & Assert
            await Assert.ThrowsAsync<EventWrongParameterException>(() => eventService.CreateEventAsync(new CreateEventDto(title, startAt, endAt, "", totalSeats), CancellationToken.None));
            mockRepository.Verify(mock => mock.AddEventAsync(It.IsAny<Event>()), Times.Never);
        }


        [Fact]
        public async Task DeleteEvent_CallWithCorrectId_CallRepoRemoveMethode()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
            Mock<IEventRepository> mockRepository = fixture.MockEventRepository;
            Guid id = Guid.NewGuid();
            string _id = id.ToString();
            mockRepository.Setup(mock => mock.RemoveEventAsync(id)).ReturnsAsync(() => true);
            // Act 
            await eventService.DeleteEventAsync(_id, CancellationToken.None);
            // Assert
            mockRepository.Verify(mock => mock.RemoveEventAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteEvent_CallWithWrongId_Exception()
        {

            // Arrange
            var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
            Mock<IEventRepository> mockRepository = fixture.MockEventRepository;
            string id = "123";  
            // Act & Assert
            await Assert.ThrowsAsync<EventWrongParameterException>(() => eventService.DeleteEventAsync(id, CancellationToken.None));
            mockRepository.Verify(mock => mock.RemoveEventAsync(new Guid()), Times.Never);
        }

        [Fact]
        public async Task DeleteEvent_NotFoundEvent_Exception()
        {

            // Arrange
            var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
            Mock<IEventRepository> mockRepository = fixture.MockEventRepository;
            Guid id = Guid.NewGuid();
            string _id = id.ToString();
            mockRepository.Setup(mock => mock.RemoveEventAsync(id)).ReturnsAsync(() => false);
            // Act & Assert
            await Assert.ThrowsAsync<EventNotFoundException>(() => eventService.DeleteEventAsync(_id, CancellationToken.None));
            mockRepository.Verify(mock => mock.RemoveEventAsync(new Guid()), Times.Never);
        }

        [Fact]
        public async Task GetEvent_CorrectId_ReturnEvent()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
            Mock<IEventRepository> mockRepository = fixture.MockEventRepository;
            Event _event = EventCollection.GetEvent(0);
            string id = _event.Id.ToString();
            mockRepository.Setup(mock => mock.GetEventAsync(_event.Id)).ReturnsAsync(_event);
            // Act
            EventInfoDto dto = await eventService.GetEventAsync(id, CancellationToken.None);
            // Assert
            Assert.NotNull(dto);
            Assert.Equal(id, dto.Id);
            Assert.Equal(_event.Title, dto.Title);
            Assert.Equal(_event.StartAt, dto.StartAt);
            Assert.Equal(_event.EndAt, dto.EndAt);
            Assert.Equal(_event.Description, dto.Description);
        }

        [Fact]
        public async Task GetEvent_WrongId_Exception()
        {
            // Arrange
            string id = "123";
            var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
            Mock<IEventRepository> mockRepository = fixture.MockEventRepository;
            // Act & Assert
            await Assert.ThrowsAsync<EventWrongParameterException>(() => eventService.GetEventAsync(id, CancellationToken.None));
            mockRepository.Verify(mock => mock.GetEventAsync(new Guid()), Times.Never);
        }

        [Fact]
        public async Task GetEvent_NotExistedId_NotFoundExceotion()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
            Mock<IEventRepository> mockRepository = fixture.MockEventRepository;
            Event _event = EventCollection.GetEvent(0);
            string id = _event.Id.ToString();
            mockRepository.Setup(mock => mock.GetEventAsync(_event.Id)).ReturnsAsync(() => null);
            // Act & Assert
            await Assert.ThrowsAsync<EventNotFoundException>(() => eventService.GetEventAsync(id, CancellationToken.None));
        }

        public static IEnumerable<object?[]> ReplaceEventCorrectArguments()
        {
            Event _event = EventCollection.GetEvent(0);
            return new List<object?[]>
            {
                new object?[] { _event.Id, _event.Title, _event.StartAt, _event.EndAt, _event.TotalSeats, _event.AvailableSeats, "Some description", "Some description" },
                new object?[] { _event.Id, _event.Title, _event.StartAt, _event.EndAt, _event.TotalSeats, _event.AvailableSeats, "", "" },
                new object?[] { _event.Id, _event.Title, _event.StartAt, _event.EndAt, _event.TotalSeats, _event.AvailableSeats, null, "" }
            };
        }
        [Theory]
        [MemberData(nameof(ReplaceEventCorrectArguments))]
        public async Task ReplaceEvent_CorrectArguments_ReturnEvent(Guid id, string title, DateTime? startAt, DateTime? endAt, int? totalSeats, int? availableSeats, string? description, string expectedDescription)
        {
            // Arrange
            var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
            Mock<IEventRepository> mockRepository = fixture.MockEventRepository;
            EventInfoDto dto = new (id.ToString(), title, startAt, endAt, description, totalSeats, availableSeats);
            mockRepository.Setup(mock => mock.ReplaceEventAsync(It.Is<Event>(e =>  e.Id == id &&
                                                                              e.Title == title &&
                                                                              e.StartAt == startAt &&
                                                                              e.EndAt == endAt &&
                                                                              e.TotalSeats == totalSeats &&
                                                                              e.AvailableSeats == availableSeats &&
                                                                              e.Description == expectedDescription))).ReturnsAsync(() => true);
            // Act
            await eventService.ReplaceEventAsync(dto.Id, dto, CancellationToken.None);
            // Assert
            mockRepository.Verify(mock => mock.ReplaceEventAsync(It.IsAny<Event>()), Times.AtMost(3));
        }

        public static IEnumerable<object?[]> Get_ReplaceEvent_WrongArguments()
        {
            EventInfoDto dto = EventCollection.GetEventDto(0);
            string unequalId = "349b6818-0d33-43ed-94e4-84824b09eeee";
            string wrongFormatId = "349";
            return new List<object?[]>
            {
                new object?[] { wrongFormatId, wrongFormatId, "", dto.StartAt, dto.EndAt, dto.TotalSeats, dto.AvailableSeats },
                new object?[] { unequalId, dto.Id, "", dto.StartAt, dto.EndAt, dto.TotalSeats, dto.AvailableSeats },
                new object?[] { dto.Id, dto.Id, "", dto.StartAt, dto.EndAt, dto.TotalSeats, dto.AvailableSeats },
                new object?[] { dto.Id, dto.Id, dto.Title, null, dto.EndAt, dto.TotalSeats, dto.AvailableSeats },
                new object?[] { dto.Id, dto.Id, dto.Title, dto.StartAt, null, dto.TotalSeats, dto.AvailableSeats },
                new object?[] { dto.Id, dto.Id, dto.Title, dto.StartAt, dto.EndAt, 0, 0 },
                new object?[] { dto.Id, dto.Id, dto.Title, dto.StartAt, dto.EndAt, null, 0 },
                new object?[] { dto.Id, dto.Id, dto.Title, dto.StartAt, dto.EndAt, dto.TotalSeats, null },
                new object?[] { dto.Id, dto.Id, dto.Title, dto.StartAt, dto.EndAt, dto.TotalSeats, -1 },
                new object?[] { dto.Id, dto.Id, "", null, null, null, null }
            };
        }


        [Theory]
        [MemberData(nameof(Get_ReplaceEvent_WrongArguments))]
        public async Task ReplaceEvent_CallWithWrongArguments_Exception(string idFromRout, string id, string title, DateTime? startAt, DateTime? endAt, int? totalSeats, int? availableSeats)
        {
            //Arrange
            var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
            Mock<IEventRepository> mockRepository = fixture.MockEventRepository;

            //Act & Assert
            await Assert.ThrowsAsync<EventWrongParameterException>(() => eventService.ReplaceEventAsync(idFromRout, new EventInfoDto(id, title, startAt, endAt, "", totalSeats, availableSeats), CancellationToken.None));
            mockRepository.Verify(mock => mock.ReplaceEventAsync(It.IsAny<Event>()), Times.Never);
        }

        [Fact]
        public async Task ReplaceEvent_NotFoundEvent_Exception()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
            Mock<IEventRepository> mockRepository = fixture.MockEventRepository;
            EventInfoDto dto = EventCollection.GetEventDto(0);
            mockRepository.Setup(mock => mock.ReplaceEventAsync(It.IsAny<Event>())).ReturnsAsync(() => false);
            // Act
            await Assert.ThrowsAsync<EventNotFoundException>(() => eventService.ReplaceEventAsync(dto.Id, dto, CancellationToken.None));
            // Assert
            mockRepository.Verify(mock => mock.ReplaceEventAsync(It.IsAny<Event>()), Times.Once);
        }

        public static IEnumerable<object?[]> Get_GetEvents_CorrectArguments()
        {             
            IQueryable<Event> events = EventCollection.GetCollection().AsQueryable<Event>();
            List<EventInfoDto> eventsDto = EventCollection.GetDtoCollection();
            return new List<object?[]>
            {
                new object?[] { null, null, null, 1, 10, events,
                    new PaginatedResultDto() { TotalResultsNumber = eventsDto.Count, CurrentPage = 1, ResultsNumberOnPage = 10, ResultsOnPage = eventsDto.GetRange(0, 10)} },
                new object?[] { "Event", null, null, 1, 10, events,
                    new PaginatedResultDto() { TotalResultsNumber = 2, CurrentPage = 1, ResultsNumberOnPage = 2, ResultsOnPage = eventsDto.GetRange(10, 2)} },
                new object?[] { null, new DateTime(2025, 06, 10), new DateTime(2026, 12, 20), 3, 4, events,
                    new PaginatedResultDto() { TotalResultsNumber = 9, CurrentPage = 3, ResultsNumberOnPage = 1, ResultsOnPage = eventsDto.GetRange(9, 1)} },
                new object?[] { "фести", new DateTime(2025, 06, 10), new DateTime(2026, 12, 20), 1, 10, events,
                    new PaginatedResultDto() { TotalResultsNumber = 2, CurrentPage = 1, ResultsNumberOnPage = 2, ResultsOnPage = [ eventsDto[1], eventsDto[5] ] } }
            };
        }

        [Theory]
        [MemberData(nameof(Get_GetEvents_CorrectArguments))]
        public async Task GetEvents_CorrectArguments_ReturnPaginatedResult(string? title, DateTime? from, DateTime? to, int page, int size, IQueryable<Event> events, PaginatedResultDto expectedResult)
        {
            // Arrange
            var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
            Mock<IEventRepository> mockRepository = fixture.MockEventRepository;
            mockRepository.Setup(mock => mock.GetEventsAsync()).ReturnsAsync(events);
            // Act && Assert
            var result = await eventService.GetEventsAsync(title, from, to, page, size, CancellationToken.None);
            Assert.Equal(result, expectedResult);
        }


        public static IEnumerable<object?[]> Get_GetEvents_WrongArguments()
        {
            List<Event> events = EventCollection.GetCollection();
            List<EventInfoDto> eventsDto = EventCollection.GetDtoCollection();
            return new List<object?[]>
            {
                new object?[] { null, new DateTime(2025, 06, 10), new DateTime(2024, 06, 10), 1, 10 },
                new object?[] { null, null, null, 0, 10 },
                new object?[] { null, null, null, 1, 0 }
            };
        }

        [Theory]
        [MemberData(nameof(Get_GetEvents_WrongArguments))]
        public async Task GetEvents_WrongArguments_Exception(string? title, DateTime? from, DateTime? to, int page, int size)
        {
            // Arrange
            var fixture = new EventServiceFixture();
            EventService eventService = fixture.EventService;
            Mock<IEventRepository> mockRepository = fixture.MockEventRepository;
            // Act & Assert 
            await Assert.ThrowsAsync<EventWrongParameterException>(() => eventService.GetEventsAsync(title, from, to, page, size, CancellationToken.None));
            mockRepository.Verify(mock => mock.GetEventsAsync(), Times.Never);
        }
    }
}

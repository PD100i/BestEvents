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

    public class EventServiceTest(EventServiceFixture fixture): IClassFixture<EventServiceFixture>
    {
        private readonly EventService eventService = fixture.EventService;
        private readonly Mock<IEventRepository> mockRepository = fixture.MockEventRepository;

        public static IEnumerable<object?[]> GetEventCorrectArguments()
        {
            Event _event = EventCollection.GetEvent(0);
            return new List<object?[]>
            {
                new object?[] { _event.Id, _event.Title, _event.StartAt, _event.EndAt, "Some description", "Some description" },
                new object?[] { _event.Id, _event.Title, _event.StartAt, _event.EndAt, "", "" },
                new object?[] { _event.Id, _event.Title, _event.StartAt, _event.EndAt, null, "" }
            };
        }

        [Theory]
        [MemberData(nameof(GetEventCorrectArguments))]
        public void CreateEvent_CallWithCorrectArguments_CallRepoMethodReturnEvent(Guid id, string title, DateTime? startAt, DateTime? endAt, string? description, string expectedDescription)
        {
            var _event = new Event(id, title, startAt, endAt, description);


            mockRepository.Setup(mock => mock.AddEvent(It.Is<Event>(e => e.Title == title &&
                                                                              e.StartAt == startAt &&
                                                                              e.EndAt == endAt &&
                                                                              e.Description == expectedDescription)))
                                                                              .Returns(_event);

            EventDto result = eventService.CreateEvent(new EventDtoBase(title, startAt, endAt, description));

            Assert.NotNull(result);
            Assert.Equal(result.Id, id.ToString());
            Assert.Equal(result.Title, title);
            Assert.Equal(result.StartAt, startAt);
            Assert.Equal(result.EndAt, endAt);
            Assert.Equal(result.Description, expectedDescription);
        }


        public static IEnumerable <object?[]> Get_CreateEvent_WrongArguments()
        {
            Event _event = EventCollection.GetEvent(0);
            return new List<object?[]>
            {
                 new object?[] { "", _event.StartAt, _event.EndAt },
                 new object?[] { _event.Title, null, _event.EndAt },
                 new object?[] { _event.Title, _event.StartAt, null },
                 new object?[] { "", null, null }
            };
    
        }

        [Theory]
        [MemberData(nameof(Get_CreateEvent_WrongArguments))]
        public void CreateEvent_CallWithWrongArguments_Exception(string title, DateTime? startAt, DateTime? endAt)
        {
            Assert.Throws<EventWrongParameterException>(() => eventService.CreateEvent(new EventDtoBase(title, startAt, endAt, "")));
            mockRepository.Verify(mock => mock.AddEvent(It.IsAny<Event>()), Times.Never);
        }


        [Fact]
        public void DeleteEvent_CallWithCorrectId_CallRepoRemoveMethode()
        {
            Guid id = Guid.NewGuid();
            string _id = id.ToString();
            mockRepository.Setup(mock => mock.RemoveEvent(id));
            eventService.DeleteEvent(_id);
        }

        [Fact]
        public void DeleteEvent_CallWithWrongId_Exception()
        {
            string id = "123";  
            Assert.Throws<EventWrongParameterException>(() => eventService.DeleteEvent(id));
            mockRepository.Verify(mock => mock.RemoveEvent(new Guid()), Times.Never);
        }

        [Fact]
        public void GetEvent_CorrectId_ReturnEvent()
        {
            Event _event = EventCollection.GetEvent(0);
            string id = _event.Id.ToString();
            mockRepository.Setup(mock => mock.GetEvent(_event.Id)).Returns(_event);
            EventDto dto = eventService.GetEvent(id);
            Assert.NotNull(dto);
            Assert.Equal(id, dto.Id);
            Assert.Equal(_event.Title, dto.Title);
            Assert.Equal(_event.StartAt, dto.StartAt);
            Assert.Equal(_event.EndAt, dto.EndAt);
            Assert.Equal(_event.Description, dto.Description);
        }

        [Fact]
        public void GetEvent_WrongId_Exception()
        {
            string id = "123";
            Assert.Throws<EventWrongParameterException>(() => eventService.GetEvent(id));
            mockRepository.Verify(mock => mock.GetEvent(new Guid()), Times.Never);
        }

        [Fact]
        public void GetEvent_NotExistedId_NotFoundExceotion()
        {
            Event _event = EventCollection.GetEvent(0);
            string id = _event.Id.ToString();
            mockRepository.Setup(mock => mock.GetEvent(_event.Id)).Throws(new DataNotFoundException(""));
            Assert.Throws<DataNotFoundException>(() => eventService.GetEvent(id));
        }

        [Theory]
        [MemberData(nameof(GetEventCorrectArguments))]
        public void ReplaceEvent_CorrectArguments_ReturnEvent(Guid id, string title, DateTime? startAt, DateTime? endAt, string? description, string expectedDescription)
        {
            EventDto dto = new (id.ToString(), title, startAt, endAt, description);
            mockRepository.Setup(mock => mock.ReplaceEvent(It.Is<Event>(e => e.Id == id &&
                                                                              e.Title == title &&
                                                                              e.StartAt == startAt &&
                                                                              e.EndAt == endAt &&
                                                                              e.Description == expectedDescription)));
            eventService.ReplaceEvent(dto.Id, dto);
            
        }

        public static IEnumerable<object?[]> Get_ReplaceEvent_WrongArguments()
        {
            EventDto dto = EventCollection.GetEventDto(0);
            return new List<object?[]>
            {
                new object?[] { "349", "349", "", dto.StartAt, dto.EndAt },
                new object?[] { "349b6818-0d33-43ed-94e4-84824b09eeee", dto.Id, "", dto.StartAt, dto.EndAt },
                new object?[] { dto.Id, dto.Id, "", dto.StartAt, dto.EndAt },
                new object?[] { dto.Id, dto.Id, dto.Title, null, dto.EndAt },
                new object?[] { dto.Id, dto.Id, dto.Title, dto.StartAt, null },
                new object?[] { dto.Id, dto.Id, "", null, null }
            };
        }

        [Theory]
        [MemberData(nameof(Get_ReplaceEvent_WrongArguments))]
        public void ReplaceEvent_CallWithWrongArguments_Exception(string idFromRout, string id, string title, DateTime? startAt, DateTime? endAt)
        {            
            Assert.Throws<EventWrongParameterException>(() => eventService.ReplaceEvent(idFromRout, new EventDto(id, title, startAt, endAt, "")));
            mockRepository.Verify(mock => mock.ReplaceEvent(It.IsAny<Event>()), Times.Never);
        }

        public static IEnumerable<object?[]> Get_GetEvents_CorrectArguments()
        {             
            List<Event> events = EventCollection.GetCollection();
            List<EventDto> eventsDto = EventCollection.GetDtoCollection();
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
        public void GetEvents_CorrectArguments_ReturnPaginatedResult(string? title, DateTime? from, DateTime? to, int page, int size, List<Event> events, 
            PaginatedResultDto expectedResult)
        {
            mockRepository.Setup(mock => mock.GetEvents()).Returns(events);
            var result = eventService.GetEvents(title, from, to, page, size);
            Assert.Equal(result, expectedResult);
        }


        public static IEnumerable<object?[]> Get_GetEvents_WrongArguments()
        {
            List<Event> events = EventCollection.GetCollection();
            List<EventDto> eventsDto = EventCollection.GetDtoCollection();
            return new List<object?[]>
            {
                new object?[] { null, new DateTime(2025, 06, 10), new DateTime(2024, 06, 10), 1, 10 },
                new object?[] { null, null, null, 0, 10 },
                new object?[] { null, null, null, 1, 0 }
            };
        }

        [Theory]
        [MemberData(nameof(Get_GetEvents_WrongArguments))]
        public void GetEvents_WrongArguments_Exception(string? title, DateTime? from, DateTime? to, int page, int size)
        {
            Assert.Throws<EventWrongParameterException>(() => eventService.GetEvents(title, from, to, page, size));
            mockRepository.Verify(mock => mock.GetEvents(), Times.Never);
        }
    }
}

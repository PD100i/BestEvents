using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BestEvents;
using Moq;

namespace BestEventsTest
{
    //public class EventServiceFixture
    //{
    //    public Mock<IEventRepository> MockEventRepository { get; set; }

    //    public EventService EventService { get; set; }
        
    //    public EventServiceFixture() 
    //    { 
    //        MockEventRepository = new Mock<IEventRepository>();
    //        EventService = new EventService(MockEventRepository.Object, new EventFilters(), new Pagination<Event>());
    //    }
    //}

    public class EventServiceTest //IClassFixture<EventServiceFixture>
    {
        // private readonly EventService eventService = fixture.EventService;
        // private readonly Mock<IEventRepository> mockRepository = fixture.MockEventRepository;

        public class CreateEvent_CallWithCorrectArgumentsData : IEnumerable<object?[]>
        {
            Event _event = EventCollection.GetEvent(0);
            public IEnumerator<object?[]> GetEnumerator()
            {
                yield return new object?[] { _event.Id, _event.Title, _event.StartAt, _event.EndAt, "Some description", "Some description" };
                yield return new object?[] { _event.Id, _event.Title, _event.StartAt, _event.EndAt, "", "" };
                yield return new object?[] { _event.Id, _event.Title, _event.StartAt, _event.EndAt, null, "" };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }



        [Theory]
        [ClassData(typeof(CreateEvent_CallWithCorrectArgumentsData))]
        public void CreateEvent_CallWithCorrectArguments_CallRepoMethodReturnEvent(Guid id, string title, DateTime? startAt, DateTime? endAt, string? description, string resultDescription)
        {
            var _event = new Event(id, title, startAt, endAt, description);     
            

            var mockEventRepository = new Mock<IEventRepository>();
            mockEventRepository.Setup(mock => mock.AddEvent(It.IsAny<Event>())).Returns(_event);
            var eventService = new EventService(mockEventRepository.Object, new EventFilters(), new Pagination<Event>());

            EventDto result = eventService.CreateEvent(new EventDtoBase(title, startAt, endAt, description));

            Assert.NotNull(result);
            Assert.Equal(result.Id, id.ToString());
            Assert.Equal(result.Title, title);
            Assert.Equal(result.StartAt, startAt);
            Assert.Equal(result.EndAt, endAt);
            Assert.Equal(result.Description, resultDescription);
        }

        //public void CreateEvent_WrongData_Exception()
        //{ 
        
        //}

        

    }
}

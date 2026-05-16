using BestEvents;
using BestEvents.Exceptions;
using Microsoft.EntityFrameworkCore;
using Moq;



namespace BestEventsTest
{

    public class EventServiceTest()
    {
        private Event CreateEvent()
        {
            return new Event()
            {
                Id = Guid.Parse("2f3bf53d-ee2d-4973-9aca-93f767e7d40f"),
                Title = "Весенняя ярмарка ремёсел",
                StartAt = new DateTime(2025, 04, 15),
                EndAt = new DateTime(2025, 04, 20),
                TotalSeats = 1000,
                AvailableSeats = 100
            };
        }

        [Fact]
        public async Task CreateEvent_SuccesfullWrite_ShouldReturnEvent()
        {
            // Arrange
            var _event = CreateEvent();
           
            var mockRepo = new Mock<IEventRepository>();    
            mockRepo.Setup(repo => repo.AddEventAsync(_event, CancellationToken.None)).ReturnsAsync(_event);
            var eventService = new EventService(mockRepo.Object);

            // Act
            var result = await eventService.CreateEventAsync(_event, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_event, result);
            mockRepo.Verify(repo => repo.AddEventAsync(_event, CancellationToken.None), Times.Once);
        }


        [Fact]
        public async Task DeleteEvent_ShouldCallDeleteRepoMethod()
        {
            // Arrange
            var mockRepo = new Mock<IEventRepository>();
            Guid id = Guid.NewGuid();
            mockRepo.Setup(repo => repo.DeleteEventAsync(id, CancellationToken.None));
            var eventService = new EventService(mockRepo.Object);

            // Act
            await eventService.DeleteEventAsync(id, CancellationToken.None);

            // Assert
            mockRepo.Verify(repo => repo.DeleteEventAsync(id, CancellationToken.None), Times.Once);
        }


        [Fact]
        public async Task GetEvent_ShouldCallGetEventsRepoMethod()
        {
            // Arrange
            var _event = CreateEvent();
            var mockRepo = new Mock<IEventRepository>();
            mockRepo.Setup(repo => repo.GetEventAsync(_event.Id, CancellationToken.None)).ReturnsAsync(_event);
            var eventService = new EventService(mockRepo.Object);

            // Act
            var result = await eventService.GetEventAsync(_event.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result, _event);
            mockRepo.Verify(repo => repo.GetEventAsync(_event.Id, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task ReplaceEvent_CallWithCorrectData_ShouldCallGetReplaceRepoMethod()
        {
            // Arrange
            var _event = CreateEvent();
            var mockRepo = new Mock<IEventRepository>();
            mockRepo.Setup(repo => repo.ReplaceEventAsync(_event, CancellationToken.None));
            var eventService = new EventService(mockRepo.Object);

            // Act
            await eventService.ReplaceEventAsync(_event.Id, _event, CancellationToken.None);

            // Assert
            mockRepo.Verify(repo => repo.ReplaceEventAsync(_event, CancellationToken.None), Times.Once);

        }


        [Fact]
        public async Task ReplaceEvent_MismatchId_ShouldThrowEventWrongParameterException()
        {
            // Arrange
            var _event = CreateEvent();
            var mockRepo = new Mock<IEventRepository>();
            mockRepo.Setup(repo => repo.ReplaceEventAsync(_event, CancellationToken.None));
            var eventService = new EventService(mockRepo.Object);

            // Act & Assert
            await Assert.ThrowsAsync<EventWrongParameterException>(() => eventService.ReplaceEventAsync(Guid.NewGuid(), _event, CancellationToken.None));
            mockRepo.Verify(repo => repo.ReplaceEventAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        public async Task GetEvents_CallWithCorrectArguments_ShouldReturnPaginatedResult()
        {
            // Arrange
           
            string? title = "SomeTitle";
            DateTime? from = DateTime.UtcNow.AddDays(-2);
            DateTime? to = DateTime.UtcNow.AddDays(2);
            int page = 1;
            int size = 10;

            var mockRepo = new Mock<IEventRepository>();
            mockRepo.Setup(repo => repo.GetEventsAsync(title, from, to, page, size, CancellationToken.None));
            var eventService = new EventService(mockRepo.Object);

            // Act 
            var result = await eventService.GetEventsAsync(title, from, to, page, size, CancellationToken.None);

            // Assert
            mockRepo.Verify(repo => repo.GetEventsAsync(title, from, to, page, size, CancellationToken.None), Times.Once);
        }


    }
}

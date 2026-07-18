using Moq;
using Events.Application;
using Events.Application.Exceptions;
using Events.Domain;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Events.UnitTest
{
    public class EventServiceTest
    {
        public class EventServiceFixture
        {
            public EventService EventService { get; }
            public Mock<IEventRepository> MockRepository { get; }
            public Mock<IUnitOfWork> MockUnitOfWork { get; }
            public Mock<IEventsCache> MockCache { get; }
            public Mock<ILogger<EventService>> MockLogger { get; }

            public EventServiceFixture()
            {
                MockRepository = new Mock<IEventRepository>();
                MockUnitOfWork = new Mock<IUnitOfWork>();
                MockCache = new Mock<IEventsCache>();
                MockLogger = new Mock<ILogger<EventService>>();
                EventService = new EventService(MockRepository.Object, MockUnitOfWork.Object, MockCache.Object, MockLogger.Object);
            }

            public Event GetEvent()
            {
                return Event.CreateInstanceEvent(Guid.NewGuid(), "Title", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), "Description", 10, 10);
            }

            public void SetupDatabaseGetEvent(Guid eventId, Event? eventData)
            {
                MockRepository.Setup(repo => repo.GetEventAsync(eventId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(eventData);
            }

            public void SetupCacheGetEvent(Guid eventId, Event? eventData)
            {
                MockCache.Setup(cache => cache.GetEventAsync(eventId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(eventData);
            }
        }

        [Fact]
        public async Task GetEvent_DataChached_ShouldReturnDataFromCache()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            var eventData = fixture.GetEvent();
            fixture.SetupCacheGetEvent(eventData.Id, eventData);
            fixture.SetupDatabaseGetEvent(eventData.Id, eventData);

            // Act
            var result = await fixture.EventService.GetEventAsync(eventData.Id, CancellationToken.None);

            // Assert
            Assert.Equal(eventData, result);
            fixture.MockCache.Verify(cache => cache.GetEventAsync(eventData.Id, It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockRepository.Verify(repo => repo.GetEventAsync(eventData.Id, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetEvent_DataNotChached_ShouldReturnDataFromDatabase()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            var eventData = fixture.GetEvent();
            fixture.SetupCacheGetEvent(eventData.Id, null); 
            fixture.SetupDatabaseGetEvent(eventData.Id, eventData);
            // Act
            var result = await fixture.EventService.GetEventAsync(eventData.Id, CancellationToken.None);
            // Assert
            Assert.Equal(eventData, result);
            fixture.MockCache.Verify(cache => cache.GetEventAsync(eventData.Id, It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockRepository.Verify(repo => repo.GetEventAsync(eventData.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetEvent_DataNotFound_ShouldThrowEventNotFoundException()
        {
            var fixture = new EventServiceFixture();
            var eventId = Guid.NewGuid();
            fixture.SetupCacheGetEvent(eventId, null);  
            fixture.SetupDatabaseGetEvent(eventId, null); 

            // Act & Assert
            await Assert.ThrowsAsync<EventNotFoundException>(() => fixture.EventService.GetEventAsync(eventId, CancellationToken.None));
        }

        [Fact]
        public async Task GetEvent_CacheThrowsException_ShouldReturnDataFromDatabase()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            var eventData = fixture.GetEvent();
            fixture.MockCache.Setup(cache => cache.GetEventAsync(eventData.Id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Кэш в состоянии ошибки"));
            fixture.SetupDatabaseGetEvent(eventData.Id, eventData);
            // Act
            var result = await fixture.EventService.GetEventAsync(eventData.Id, CancellationToken.None);
            // Assert
            Assert.Equal(eventData, result);
            fixture.MockCache.Verify(cache => cache.GetEventAsync(eventData.Id, It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockRepository.Verify(repo => repo.GetEventAsync(eventData.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetEvent_DatabaseThrowsException_ShouldThrowException()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            var eventData = fixture.GetEvent();
            fixture.SetupCacheGetEvent(eventData.Id, null); // Cache returns null
            fixture.MockRepository.Setup(repo => repo.GetEventAsync(eventData.Id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("База данных в состоянии ошибки"));
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => fixture.EventService.GetEventAsync(eventData.Id, CancellationToken.None));
        }

        [Fact]
        public async Task CreateEventAsync_ShouldAddToDatabaseAndUofSaveChangesAndInvalidateCache()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            var eventData = fixture.GetEvent();
            fixture.MockCache.Setup(cache => cache.InvalidateEventAsync(eventData.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            fixture.MockUnitOfWork.Setup(uof => uof.SaveChangesAsync(It.IsAny<CancellationToken>()));
            // Act
            await fixture.EventService.CreateEventAsync(eventData, CancellationToken.None);
            // Assert
            fixture.MockRepository.Verify(repo => repo.AddEventAsync(eventData, It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockUnitOfWork.Verify(uof => uof.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockCache.Verify(cache => cache.InvalidateEventAsync(eventData.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateEventAsync_CaceThrowException_ShouldAddToDatabase()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            var eventData = fixture.GetEvent();
            fixture.MockCache.Setup(cache => cache.InvalidateEventAsync(eventData.Id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Кэш в состоянии ошибки"));
            fixture.MockUnitOfWork.Setup(uof => uof.SaveChangesAsync(It.IsAny<CancellationToken>()));

            // Act
            await fixture.EventService.CreateEventAsync(eventData, CancellationToken.None);

            // Assert
            fixture.MockRepository.Verify(repo => repo.AddEventAsync(eventData, It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockUnitOfWork.Verify(uof => uof.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteEventAsync_ShouldDeleteFromDatabaseAndUofSaveChangesAndInvalidateCache()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            var eventData = fixture.GetEvent();
            fixture.SetupDatabaseGetEvent(eventData.Id, eventData);
            fixture.MockCache.Setup(cache => cache.InvalidateEventAsync(eventData.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            fixture.MockUnitOfWork.Setup(uof => uof.SaveChangesAsync(It.IsAny<CancellationToken>()));

            // Act
            await fixture.EventService.DeleteEventAsync(eventData.Id, CancellationToken.None);

            // Assert
            fixture.MockRepository.Verify(repo => repo.DeleteEventAsync(eventData.Id, It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockUnitOfWork.Verify(uof => uof.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockCache.Verify(cache => cache.InvalidateEventAsync(eventData.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteEventAsync_CacheThrowException_ShouldDeleteFromDatabase()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            var eventData = fixture.GetEvent();
            fixture.SetupDatabaseGetEvent(eventData.Id, eventData);
            fixture.MockCache.Setup(cache => cache.InvalidateEventAsync(eventData.Id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Кэш в состоянии ошибки"));
            fixture.MockUnitOfWork.Setup(uof => uof.SaveChangesAsync(It.IsAny<CancellationToken>()));
            // Act
            await fixture.EventService.DeleteEventAsync(eventData.Id, CancellationToken.None);
            // Assert
            fixture.MockRepository.Verify(repo => repo.DeleteEventAsync(eventData.Id, It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockUnitOfWork.Verify(uof => uof.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteEventAsync_EventNotFound_ShouldThrowEventNotFoundException()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            var eventId = Guid.NewGuid();
            fixture.MockRepository.Setup(repo => repo.DeleteEventAsync(eventId, It.IsAny<CancellationToken>())).ThrowsAsync(new EventNotFoundException(string.Format(Messages_ru.EventNotFound, eventId)));
            // Act & Assert
            await Assert.ThrowsAsync<EventNotFoundException>(() => fixture.EventService.DeleteEventAsync(eventId, CancellationToken.None));
        }

        [Fact]
        public async Task ReplaceEventAsync_ShouldReplaceInDatabaseAndUofSaveChangesAndInvalidateCache()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            var eventData = fixture.GetEvent();
            fixture.SetupDatabaseGetEvent(eventData.Id, eventData);
            fixture.MockCache.Setup(cache => cache.InvalidateEventAsync(eventData.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            fixture.MockUnitOfWork.Setup(uof => uof.SaveChangesAsync(It.IsAny<CancellationToken>()));
            // Act
            await fixture.EventService.ReplaceEventAsync(eventData.Id, eventData, CancellationToken.None);
            // Assert
            fixture.MockRepository.Verify(repo => repo.ReplaceEventAsync(eventData, It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockUnitOfWork.Verify(uof => uof.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockCache.Verify(cache => cache.InvalidateEventAsync(eventData.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReplaceEventAsync_CacheThrowException_ShouldReplaceInDatabase()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            var eventData = fixture.GetEvent();
            fixture.SetupDatabaseGetEvent(eventData.Id, eventData);
            fixture.MockCache.Setup(cache => cache.InvalidateEventAsync(eventData.Id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Кэш в состоянии ошибки"));
            fixture.MockUnitOfWork.Setup(uof => uof.SaveChangesAsync(It.IsAny<CancellationToken>()));
            // Act
            await fixture.EventService.ReplaceEventAsync(eventData.Id, eventData, CancellationToken.None);
            // Assert
            fixture.MockRepository.Verify(repo => repo.ReplaceEventAsync(eventData, It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockUnitOfWork.Verify(uof => uof.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReplaceEventAsync_EventNotFound_ShouldThrowEventNotFoundException()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            var eventData = fixture.GetEvent();
            fixture.MockRepository.Setup(repo => repo.ReplaceEventAsync(eventData, It.IsAny<CancellationToken>())).ThrowsAsync(new EventNotFoundException(string.Format(Messages_ru.EventNotFound, eventData.Id)));
            // Act & Assert
            await Assert.ThrowsAsync<EventNotFoundException>(() => fixture.EventService.ReplaceEventAsync(eventData.Id, eventData, CancellationToken.None));
        }

        [Fact]
        public async Task GetTopPopularEventsAsync_ShouldReturnListOfEventsFromCache()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            var eventsList = new List<Event> { fixture.GetEvent(), fixture.GetEvent() };
            fixture.MockCache.Setup(cache => cache.GetTopPopularEventsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventsList);
            fixture.MockRepository.Setup(repo => repo.GetTopPopularEventsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventsList);
            // Act
            var result = await fixture.EventService.GetTopPopularEventsAsync(CancellationToken.None);
            // Assert
            Assert.Equal(eventsList, result);
            fixture.MockCache.Verify(cache => cache.GetTopPopularEventsAsync(It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockRepository.Verify(repo => repo.GetTopPopularEventsAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetTopPopularEventsAsync_CacheThrowsException_ShouldReturnListOfEventsFromRepository()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            var eventsList = new List<Event> { fixture.GetEvent(), fixture.GetEvent() };
            fixture.MockRepository.Setup(repo => repo.GetTopPopularEventsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventsList);
            fixture.MockCache.Setup(cache => cache.GetTopPopularEventsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Кэш в состоянии ошибки"));
            // Act
            var result = await fixture.EventService.GetTopPopularEventsAsync(CancellationToken.None);
            // Assert
            Assert.Equal(eventsList, result);
            fixture.MockCache.Verify(cache => cache.GetTopPopularEventsAsync(It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockRepository.Verify(repo => repo.GetTopPopularEventsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTopPopularEventsAsync_RepositoryThrowsException_ShouldThrowException()
        {
            // Arrange
            var fixture = new EventServiceFixture();
            fixture.MockCache.Setup(cache => cache.GetTopPopularEventsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Кэш в состоянии ошибки"));
            fixture.MockRepository.Setup(repo => repo.GetTopPopularEventsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("База данных в состоянии ошибки"));
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => fixture.EventService.GetTopPopularEventsAsync(CancellationToken.None));
        }

        
    }
}
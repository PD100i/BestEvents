using Moq;
using Events.Application;
using Events.Domain;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Microsoft.Extensions.Options;
using Events.Infrastructure;

namespace Events.UnitTest
{
    public class EventsCacheTest
    {
        public class EventCacheTestFixture
        {

            public Mock<IDatabase> MockRedisDatabase { get; }           

            public Mock<IEventRepository> MockEventRepository { get; }

            public EventsCache EventsCache { get; }

            public EventCacheTestFixture()
            {
                // Инициализация основных моков
                MockRedisDatabase = new Mock<IDatabase>();
                MockEventRepository = new Mock<IEventRepository>();

                // Настройка мультиплексора Redis
                var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
                mockConnectionMultiplexer
                    .Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                    .Returns(MockRedisDatabase.Object);

                // Настройка Service Scope
                var mockServiceFactory = new Mock<IServiceScopeFactory>();
                var scope = new Mock<IServiceScope>();
                var provider = new Mock<IServiceProvider>(); // Было: provaider

                //  Мокаем GetService
                provider
                    .Setup(p => p.GetService(typeof(IEventRepository)))
                    .Returns(MockEventRepository.Object);

                scope.Setup(s => s.ServiceProvider).Returns(provider.Object);
                mockServiceFactory.Setup(f => f.CreateScope()).Returns(scope.Object);

                // Настройка опций конфигурации
                IOptions<RedisSettings> options = Options.Create(new RedisSettings());
               
                // Создание тестируемого класса
                EventsCache = new EventsCache(mockConnectionMultiplexer.Object, mockServiceFactory.Object, options);
            }

            public Event GetEvent()
            {
                return Event.CreateInstanceEvent(Guid.NewGuid(), "Title", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), "Description", 10, 8);
            }

            public string GetEventJson(Event _event)
            {
                return System.Text.Json.JsonSerializer.Serialize(_event);
            }
        }

        [Fact]
        public async Task GetEventAsync_Hit_ShouldReturnEventFromCache()
        {
            // Arrange
            var fixture = new EventCacheTestFixture();
            var _event = fixture.GetEvent();
            RedisValue cache = fixture.GetEventJson(_event);
            fixture.MockRedisDatabase.Setup(db => db.StringGetAsync($"event:{_event.Id}", It.IsAny<CommandFlags>())).ReturnsAsync(cache);

            // Act
            var result = await fixture.EventsCache.GetEventAsync(_event.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_event.Id, result.Id);
            fixture.MockEventRepository.Verify(repo => repo.GetEventAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetEventAsync_Miss_ShouldReturnEventFromRepositoryAndCacheIt()
        {
            // Arrange
            var fixture = new EventCacheTestFixture();
            var _event = fixture.GetEvent();
            RedisValue cache = RedisValue.Null;
            fixture.MockRedisDatabase.Setup(db => db.StringGetAsync($"event:{_event.Id}", It.IsAny<CommandFlags>())).ReturnsAsync(cache);
            fixture.MockEventRepository.Setup(repo => repo.GetEventAsync(_event.Id, It.IsAny<CancellationToken>())).ReturnsAsync(_event);
            // Act
            var result = await fixture.EventsCache.GetEventAsync(_event.Id, CancellationToken.None);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(_event.Id, result.Id);
            fixture.MockEventRepository.Verify(repo => repo.GetEventAsync(_event.Id, It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockRedisDatabase.Verify(db => db.StringSetAsync(
                $"event:{_event.Id}",
                It.IsAny<RedisValue>(),
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()
            ), Times.Once);
        }

        [Fact]
        public async Task InvalidateEventAsync_ShouldDeleteEventFromCache()
        {
            // Arrange
            var fixture = new EventCacheTestFixture();
            var _event = fixture.GetEvent();
            // Act
            await fixture.EventsCache.InvalidateEventAsync(_event.Id, CancellationToken.None);
            // Assert
            fixture.MockRedisDatabase.Verify(db => db.KeyDeleteAsync($"event:{_event.Id}", It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public async Task GetTopPopularEventsAsync_Hit_ShouldReturnEventsFromCache()
        {
            // Arrange
            var fixture = new EventCacheTestFixture();
            var events = new List<Event> { fixture.GetEvent(), fixture.GetEvent() };
            RedisValue cache = System.Text.Json.JsonSerializer.Serialize(events);
            fixture.MockRedisDatabase.Setup(db => db.StringGetAsync("events:top", It.IsAny<CommandFlags>())).ReturnsAsync(cache);
            // Act
            var result = await fixture.EventsCache.GetTopPopularEventsAsync(CancellationToken.None);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(events.Count, result.Count);
            fixture.MockEventRepository.Verify(repo => repo.GetTopPopularEventsAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetTopPopularEventsAsync_Miss_ShouldReturnEventsFromRepositoryAndCacheIt()
        {
            // Arrange
            var fixture = new EventCacheTestFixture();
            var events = new List<Event> { fixture.GetEvent(), fixture.GetEvent() };
            RedisValue cache = RedisValue.Null;
            fixture.MockRedisDatabase.Setup(db => db.StringGetAsync("events:top", It.IsAny<CommandFlags>())).ReturnsAsync(cache);
            fixture.MockEventRepository.Setup(repo => repo.GetTopPopularEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(events);
            // Act
            var result = await fixture.EventsCache.GetTopPopularEventsAsync(CancellationToken.None);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(events.Count, result.Count);
            fixture.MockEventRepository.Verify(repo => repo.GetTopPopularEventsAsync(It.IsAny<CancellationToken>()), Times.Once);
            fixture.MockRedisDatabase.Verify(db => db.StringSetAsync(
                "events:top",
                It.IsAny<RedisValue>(),
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()
            ), Times.Once);
        }

        [Fact]
        public async Task GetEventAsync_ShouldReturnNullIfEventNotFound()
        {
            // Arrange
            var fixture = new EventCacheTestFixture();
            var eventId = Guid.NewGuid();
            RedisValue cache = RedisValue.Null;
            fixture.MockRedisDatabase.Setup(db => db.StringGetAsync($"event:{eventId}", It.IsAny<CommandFlags>())).ReturnsAsync(cache);
            fixture.MockEventRepository.Setup(repo => repo.GetEventAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync((Event?)null);
            // Act
            var result = await fixture.EventsCache.GetEventAsync(eventId, CancellationToken.None);
            // Assert
            Assert.Null(result);
            fixture.MockEventRepository.Verify(repo => repo.GetEventAsync(eventId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTopPopularEventsAsync_ShouldReturnEmptyListIfNoEventsFound()
        {
            // Arrange
            var fixture = new EventCacheTestFixture();
            RedisValue cache = RedisValue.Null;
            fixture.MockRedisDatabase.Setup(db => db.StringGetAsync("events:top", It.IsAny<CommandFlags>())).ReturnsAsync(cache);
            fixture.MockEventRepository.Setup(repo => repo.GetTopPopularEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Event>());
            // Act
            var result = await fixture.EventsCache.GetTopPopularEventsAsync(CancellationToken.None);
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            fixture.MockEventRepository.Verify(repo => repo.GetTopPopularEventsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

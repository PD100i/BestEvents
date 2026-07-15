using Events.Application;
using Events.Domain;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Events.Application.Exceptions;

namespace Events.Infrastructure
{
    public class EventsCache(IConnectionMultiplexer connectionMultiplexery, IServiceScopeFactory scopeFactory) : IEventsCache
    {
        

        public async Task<Event> GetEventAsync(Guid id, CancellationToken ct = default)
        {
            string key = $"event:{id}";
            var db = connectionMultiplexery.GetDatabase();
            RedisValue cache = await db.StringGetAsync(key);
            if (cache.HasValue)
            {
                var _event = System.Text.Json.JsonSerializer.Deserialize<Event>(cache.ToString());
                if (_event == null) 
                    throw new EventNotFoundException(string.Format(Messages_ru.EventNotFound, id));
                return _event;
            }
            else
            {
                using var scope = scopeFactory.CreateScope();
                var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
                var _event = await eventRepository.GetEventAsync(id, ct);
                await db.StringSetAsync(key, System.Text.Json.JsonSerializer.Serialize(_event), TimeSpan.FromSeconds(5));
                return _event;
            }
        }

        public async Task<List<Event>> GetTopPopularEventsAsync(CancellationToken ct = default)
        {
            const string key = "events:top";
            var db = connectionMultiplexery.GetDatabase();
            RedisValue cache = await db.StringGetAsync(key);
            if (cache.HasValue)
            {
                var events = System.Text.Json.JsonSerializer.Deserialize<List<Event>>(cache.ToString());
                return events ?? [];
            }
            else
            {
                using var scope = scopeFactory.CreateScope();
                var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
                var events = await eventRepository.GetTopPopularEventsAsync(ct);
                await db.StringSetAsync(key, System.Text.Json.JsonSerializer.Serialize(events), TimeSpan.FromMinutes(1));
                return events;
            }

        }

        public async Task InvalidateEventAsync(Guid id, CancellationToken ct = default)
        {
            string key = $"event:{id}";
            var db = connectionMultiplexery.GetDatabase();
            await db.KeyDeleteAsync(key); 
        }
    }
}

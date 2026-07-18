using Events.Application;
using Events.Domain;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Microsoft.Extensions.Options;

namespace Events.Infrastructure
{
    public class EventsCache(IConnectionMultiplexer connectionMultiplexery, IServiceScopeFactory scopeFactory, IOptions<RedisSettings> redisSettings) : IEventsCache
    {
        private const string topEventsKey = "events:top";
        private const string eventKey = "event";

        /// <inheritdoc/>
        public async Task<Event?> GetEventAsync(Guid id, CancellationToken ct = default)
        {
            var db = connectionMultiplexery.GetDatabase();
            RedisValue cache = await db.StringGetAsync($"{eventKey}:{id}");
            if (cache.HasValue)
            {
                var _event = System.Text.Json.JsonSerializer.Deserialize<Event>(cache.ToString());
                return _event;
            }
            else
            {
                using var scope = scopeFactory.CreateScope();
                var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
                var _event = await eventRepository.GetEventAsync(id, ct);
                await db.StringSetAsync($"{eventKey}:{id}", System.Text.Json.JsonSerializer.Serialize(_event), TimeSpan.FromSeconds(redisSettings.Value.GetEvntTTL_sec));
                return _event;
            }
        }

        /// <inheritdoc/>
        public async Task<List<Event>> GetTopPopularEventsAsync(CancellationToken ct = default)
        {
            var db = connectionMultiplexery.GetDatabase();
            RedisValue cache = await db.StringGetAsync(topEventsKey);
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
                await db.StringSetAsync(topEventsKey, System.Text.Json.JsonSerializer.Serialize(events), TimeSpan.FromSeconds(redisSettings.Value.GetTopEvntsTTL_sec));
                return events;
            }

        }

        /// <inheritdoc/>
        public async Task InvalidateEventAsync(Guid id, CancellationToken ct = default)
        {
            var db = connectionMultiplexery.GetDatabase();
            await db.KeyDeleteAsync($"{eventKey}:{id}"); 
        }
    }
}

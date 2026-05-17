using BestEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEventsIntegrationTest
{
    internal static class EventCollection
    {
        private readonly static List<EventEntity> initialData = [
            new EventEntity(){ Id = Guid.Parse("2f3bf53d-ee2d-4973-9aca-93f767e7d40f"), Title = "Весенняя ярмарка ремёсел", StartAt = DateTime.SpecifyKind(new DateTime(2025, 04, 15), DateTimeKind.Utc), EndAt = DateTime.SpecifyKind(new DateTime(2025, 04, 20), DateTimeKind.Utc), TotalSeats = 1000, AvailableSeats = 1000 },
            new EventEntity() { Id = Guid.Parse("349b6818-0d33-43ed-94e4-84824b09eeee"), Title = "Международный кинофестиваль", StartAt = DateTime.SpecifyKind(new DateTime(2025, 06, 10), DateTimeKind.Utc), EndAt = DateTime.SpecifyKind(new DateTime(2025, 06, 17), DateTimeKind.Utc), TotalSeats = 1000, AvailableSeats = 1000 },
            new EventEntity() { Id = Guid.Parse("baca1455-19c1-4854-ab5d-d4362713430e"), Title = "Конференция по инновационным технологиям", StartAt = DateTime.SpecifyKind(new DateTime(2025, 08, 05), DateTimeKind.Utc), EndAt = DateTime.SpecifyKind(new DateTime(2025, 08, 07), DateTimeKind.Utc), TotalSeats = 1000, AvailableSeats = 1000 },
            new EventEntity() { Id = Guid.Parse("2df37d1a-587e-4f9f-a2b5-148c660c393a"), Title = "Городской марафон", StartAt = DateTime.SpecifyKind(new DateTime(2025, 08, 06), DateTimeKind.Utc), EndAt = DateTime.SpecifyKind(new DateTime(2025, 08, 06), DateTimeKind.Utc), TotalSeats = 1000, AvailableSeats = 1000 },
            new EventEntity() { Id = Guid.Parse("4f809853-a822-47b1-9e87-344d949d3b75"), Title = "Выставка современного искусства", StartAt = DateTime.SpecifyKind(new DateTime(2025, 11, 01), DateTimeKind.Utc), EndAt = DateTime.SpecifyKind(new DateTime(2025, 11, 15), DateTimeKind.Utc), TotalSeats = 1000, AvailableSeats = 1000 },
            new EventEntity() { Id = Guid.Parse("0d83ea08-64f8-4416-bf07-e86f53b33e53"), Title = "Фестиваль уличной еды", StartAt = DateTime.SpecifyKind(new DateTime(2026, 03, 10), DateTimeKind.Utc), EndAt = DateTime.SpecifyKind(new DateTime(2026, 03, 12), DateTimeKind.Utc), TotalSeats = 1000, AvailableSeats = 1000 },
            new EventEntity() { Id = Guid.Parse("8424113a-b4f7-4008-baad-a648c2c437e7"), Title = "Семинар по цифровой грамотности", StartAt = DateTime.SpecifyKind(new DateTime(2026, 05, 18), DateTimeKind.Utc), EndAt = DateTime.SpecifyKind(new DateTime(2026, 05, 19), DateTimeKind.Utc), TotalSeats = 1000, AvailableSeats = 1000 },
            new EventEntity() { Id = Guid.Parse("835ccd17-31b3-419d-b114-e8e1e119121a"), Title = "Музыкальный open‑air", StartAt = DateTime.SpecifyKind(new DateTime(2026, 07, 25), DateTimeKind.Utc), EndAt = DateTime.SpecifyKind(new DateTime(2026, 07, 27), DateTimeKind.Utc), TotalSeats = 1000, AvailableSeats = 1000 },
            new EventEntity() { Id = Guid.Parse("e07d69d8-11a1-400d-8dab-c1691c5b4c97"), Title = "Городская книжная ярмарка", StartAt = DateTime.SpecifyKind(new DateTime(2026, 10, 05), DateTimeKind.Utc), EndAt = DateTime.SpecifyKind(new DateTime(2026, 10, 10), DateTimeKind.Utc), TotalSeats = 1000, AvailableSeats = 1000 },
            new EventEntity() { Id = Guid.Parse("3c427551-9817-4055-a49a-fbc4f3a8d89e"), Title = "Новогодний благотворительный концерт", StartAt = DateTime.SpecifyKind(new DateTime(2026, 12, 20), DateTimeKind.Utc), EndAt = DateTime.SpecifyKind(new DateTime(2026, 12, 20), DateTimeKind.Utc), TotalSeats = 1000, AvailableSeats = 1000 },
            new EventEntity() { Id = Guid.Parse("8425b848-2d84-45d5-bb18-5566d5b7a5da"), Title = "Event1", StartAt = DateTime.SpecifyKind(new DateTime(2026, 12, 21), DateTimeKind.Utc), EndAt = DateTime.SpecifyKind(new DateTime(2026, 12, 21), DateTimeKind.Utc), TotalSeats = 1000, AvailableSeats = 1000 },
            new EventEntity() { Id = Guid.Parse("b844ef01-1db7-4d53-8933-4401cfccabeb"), Title = "Event2", StartAt = DateTime.SpecifyKind(new DateTime(2026, 12, 22), DateTimeKind.Utc), EndAt = DateTime.SpecifyKind(new DateTime(2026, 12, 22), DateTimeKind.Utc), TotalSeats = 1000, AvailableSeats = 1000 }];


        internal static List<EventEntity> GetCollection()
        {
            return [..initialData];
        }

        internal static EventEntity GetEventEntity(int index)
        {
            return GetCollection()[index];
        }


        internal static List<Event> GetEventCollection()
        {
            List<Event> events = [];
            initialData.ForEach(e => events.Add(new Event()
            {
                Id = e.Id,
                Title = e.Title,
                StartAt = e.StartAt,
                EndAt = e.EndAt,
                Description = e.Description,
                TotalSeats = e.TotalSeats,
                AvailableSeats = e.AvailableSeats
            }));
            return events;
        }
    }
}

using BestEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEventsTest
{
    internal static class EventCollection
    {
        private readonly static List<Event> initialData = [
            new Event(Guid.Parse("2f3bf53d-ee2d-4973-9aca-93f767e7d40f"), "Весенняя ярмарка ремёсел", new DateTime(2025, 04, 15), new DateTime(2025, 04, 20), ""),
            new Event(Guid.Parse("349b6818-0d33-43ed-94e4-84824b09eeee"), "Международный кинофестиваль", new DateTime(2025, 06, 10), new DateTime(2025, 06, 17), ""),
            new Event(Guid.Parse("baca1455-19c1-4854-ab5d-d4362713430e"), "Конференция по инновационным технологиям", new DateTime(2025, 08, 05), new DateTime(2025, 08, 07), ""),
            new Event(Guid.Parse("2df37d1a-587e-4f9f-a2b5-148c660c393a"), "Городской марафон", new DateTime(2025, 08, 06), new DateTime(2025, 08, 06), ""),
            new Event(Guid.Parse("4f809853-a822-47b1-9e87-344d949d3b75"), "Выставка современного искусства", new DateTime(2025, 11, 01), new DateTime(2025, 11, 15), ""),
            new Event(Guid.Parse("0d83ea08-64f8-4416-bf07-e86f53b33e53"), "Фестиваль уличной еды", new DateTime(2026, 03, 10), new DateTime(2026, 03, 12), ""),
            new Event(Guid.Parse("8424113a-b4f7-4008-baad-a648c2c437e7"), "Семинар по цифровой грамотности", new DateTime(2026, 05, 18), new DateTime(2026, 05, 19), ""),
            new Event(Guid.Parse("835ccd17-31b3-419d-b114-e8e1e119121a"), "Музыкальный open‑air", new DateTime(2026, 07, 25), new DateTime(2026, 07, 27), ""),
            new Event(Guid.Parse("e07d69d8-11a1-400d-8dab-c1691c5b4c97"), "Городская книжная ярмарка", new DateTime(2026, 10, 05), new DateTime(2026, 10, 10), ""),
            new Event(Guid.Parse("3c427551-9817-4055-a49a-fbc4f3a8d89e"), "Новогодний благотворительный концерт", new DateTime(2026, 12, 20), new DateTime(2026, 12, 20), ""),
            new Event(Guid.Parse("8425b848-2d84-45d5-bb18-5566d5b7a5da"), "Event1", new DateTime(2026, 12, 21), new DateTime(2026, 12, 21), ""),
            new Event(Guid.Parse("b844ef01-1db7-4d53-8933-4401cfccabeb"), "Event2", new DateTime(2026, 12, 22), new DateTime(2026, 12, 22), "")];


        internal static List<Event> GetCollection()
        {
            return [.. initialData];
        }

        internal static Event GetEvent(int index)
        {
            return initialData[index];
        }

        internal static EventInfo GetEventDto(int index)
        {
            Event _event = initialData[index];
            return new EventInfo(_event.Id.ToString(), _event.Title, _event.StartAt, _event.EndAt, _event.Description);
        }

        internal static List<EventInfo> GetDtoCollection()
        {
            List<EventInfo> eventsDto = [];
            initialData.ForEach(e => eventsDto.Add(new EventInfo(e.Id.ToString(), e.Title, e.StartAt, e.EndAt, e.Description)));
            return eventsDto;
        }
    }
}

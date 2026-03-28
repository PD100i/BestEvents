
using BestEvents;


namespace BestEventsTest
{
    public class EventFiltersFixture
    {
        public EventFilters EventFilters { get; set; } = new EventFilters();
        
    }

    public class EventFilterTest(EventFiltersFixture fixture) : IClassFixture<EventFiltersFixture>
    {
        private readonly static List<Event> initialData = EventCollection.GetCollection();

        private readonly EventFilters filters = fixture.EventFilters;

        public static IEnumerable<object?[]> GetFilterByTitleTestData()
        {
            return new List<object?[]>()
            {
                 new object[] { initialData, "ЕСти", new List<Event>() { initialData[1], initialData[5] } },
                 new object[] { initialData, "фестиваль уличной еды", new List<Event>() { initialData[5] } },
                 new object[] { initialData, "гор", new List<Event>() { initialData[3], initialData[8] } },
                 new object[] { initialData, "городской Марафон", new List<Event>() { initialData[3] } },
                 new object[] { initialData, "ПО", new List<Event>() { initialData[2], initialData[6] } },
                 new object[] { initialData, "ПО ИННОВ", new List<Event>() { initialData[2] } },
                 new object[] { initialData, "ААА", new List<Event>() },
                 new object[] { initialData, "", initialData },
                 new object?[] { initialData, null, initialData }
            };
        }
        [Theory]
        [MemberData(nameof(GetFilterByTitleTestData))]
        public void FilterEventByTitle_Filter_FiltredResult(List<Event> inputData, string title, List<Event> expectedResult)
        {
            var result = filters.FilterEventsByTitle(inputData, title).ToList();
            Assert.Equal(result, expectedResult);
        }

        public static IEnumerable<object?[]> GetFilterByDateFromTestData()
        {
            return new List<object?[]>()
            {
                 new object[] { initialData, new DateTime(2026, 07, 25), initialData.GetRange(7, 5) },
                 new object[] { initialData, new DateTime(2026, 07, 26), initialData.GetRange(8, 4) },
                 new object[] { initialData, new DateTime(2025, 04, 15), initialData },
                 new object[] { initialData, new DateTime(2027, 01, 01), new List<Event>() },
                 new object?[] { initialData, null, initialData }
            };
        }

        [Theory]
        [MemberData(nameof(GetFilterByDateFromTestData))]
        public void FilterEventByDateFrom_Filter_FilterResult(List<Event> inputData, DateTime? from, List<Event> expectedResult)
        {
            var result = filters.FilterEventsByDateFrom(inputData, from).ToList();
            Assert.Equal(result, expectedResult);
        }

       
        public static IEnumerable<object?[]> GetFilterByDateToTestData()
        {
            return new List<object?[]>()
            {
                new object[] { initialData, new DateTime(2025, 08, 07), initialData.GetRange(0, 4) },
                new object[] { initialData, new DateTime(2025, 04, 20), new List<Event>() { initialData[0] } },
                new object[] { initialData, new DateTime(2025, 04, 15), new List<Event>() },
                new object[] { initialData, new DateTime(2027, 01, 01), initialData },
                new object?[] { initialData, null, initialData }
            };
        }

        [Theory]
        [MemberData(nameof(GetFilterByDateToTestData))]
        public void FilterEventByDateTo_Filter_FilterResult(List<Event> inputData, DateTime? from, List<Event> expectedResult)
        {
            var result = filters.FilterEventsByDateTo(inputData, from).ToList();
            Assert.Equal(result, expectedResult);
        }
    }

    
}

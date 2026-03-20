using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public class FilterByTitleTestData : IEnumerable<object?[]>
        {
            public IEnumerator<object?[]> GetEnumerator()
            {
                yield return new object[] { initialData, "ЕСти", new List<Event>() { initialData[1], initialData[5] } };
                yield return new object[] { initialData, "фестиваль уличной еды", new List<Event>() { initialData[5] } };
                yield return new object[] { initialData, "гор", new List<Event>() { initialData[3], initialData[8] } };
                yield return new object[] { initialData, "городской Марафон", new List<Event>() { initialData[3] } };
                yield return new object[] { initialData, "ПО", new List<Event>() { initialData[2], initialData[6] } };
                yield return new object[] { initialData, "ПО ИННОВ", new List<Event>() { initialData[2] } };
                yield return new object[] { initialData, "ААА", new List<Event>() };
                yield return new object[] { initialData, "", initialData };
                yield return new object?[] { initialData, null, initialData };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Theory]
        [ClassData(typeof(FilterByTitleTestData))]
        public void FilterEventByTitle_Filter_FiltredResult(List<Event> inputData, string title, List<Event> expectedResult)
        {
            var result = filters.FilterEventsByTitle(inputData, title).ToList();
            Assert.Equal(result, expectedResult);
        }


        public class FilterByDateFromTestData : IEnumerable<object?[]>
        {
            public IEnumerator<object?[]> GetEnumerator()
            {
                yield return new object[] { initialData, new DateTime(2026, 07, 25), new List<Event>() { initialData[7], initialData[8], initialData[9] } };
                yield return new object[] { initialData, new DateTime(2026, 07, 26), new List<Event>() { initialData[8], initialData[9] } };
                yield return new object[] { initialData, new DateTime(2025, 04, 15), initialData };
                yield return new object[] { initialData, new DateTime(2027, 01, 01), new List<Event>() };
                yield return new object?[] { initialData, null, initialData };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Theory]
        [ClassData(typeof(FilterByDateFromTestData))]
        public void FilterEventByDateFrom_Filter_FilterResult(List<Event> inputData, DateTime? from, List<Event> expectedResult)
        {
            var result = filters.FilterEventsByDateFrom(inputData, from).ToList();
            Assert.Equal(result, expectedResult);
        }

        public class FilterByDateToTestData : IEnumerable<object?[]>
        {
            public IEnumerator<object?[]> GetEnumerator()
            {
                yield return new object[] { initialData, new DateTime(2025, 08, 07), new List<Event>() { initialData[0], initialData[1], initialData[2], initialData[3] } };
                yield return new object[] { initialData, new DateTime(2025, 04, 20), new List<Event>() { initialData[0] } };
                yield return new object[] { initialData, new DateTime(2025, 04, 15), new List<Event>() };
                yield return new object[] { initialData, new DateTime(2027, 01, 01), initialData };
                yield return new object?[] {initialData, null, initialData };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Theory]
        [ClassData(typeof(FilterByDateToTestData))]
        public void FilterEventByDateTo_Filter_FilterResult(List<Event> inputData, DateTime? from, List<Event> expectedResult)
        {
            var result = filters.FilterEventsByDateTo(inputData, from).ToList();
            Assert.Equal(result, expectedResult);
        }
    }

    
}

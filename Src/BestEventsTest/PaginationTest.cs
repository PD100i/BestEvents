using BestEvents;
using BestEvents.Exceptions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BestEventsTest
{
    public class PaginationFixture
    {
        public Pagination<int> Pagination { get; set; }
        public PaginationFixture() 
        { 
            Pagination = new Pagination<int>();
        }
    }

    public class PaginationTest(PaginationFixture fixture) : IClassFixture<PaginationFixture>
    {

        private static readonly List<int> inputData = [.. Enumerable.Range(1, 98)];

        private readonly Pagination<int> pagination = fixture.Pagination;

        public class CorrectPaginationTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { inputData, 1, 10, new PaginatedResult<int>([.. Enumerable.Range(1, 10)], 1, 98) };
                yield return new object[] { inputData, 2, 10, new PaginatedResult<int>([.. Enumerable.Range(11, 10)], 2, 98) };
                yield return new object[] { inputData, 10, 10, new PaginatedResult<int>([.. Enumerable.Range(91, 8)], 10, 98) };
                yield return new object[] { inputData, 11, 10, new PaginatedResult<int>([], 11, 98) };
                yield return new object[] { inputData, 1, 20, new PaginatedResult<int>([.. Enumerable.Range(1, 20)], 1, 98) };
                yield return new object[] { inputData, 5, 20, new PaginatedResult<int>([.. Enumerable.Range(81, 18)], 5, 98) };
                yield return new object[] { inputData, 6, 20, new PaginatedResult<int>([], 6, 98) };
                yield return new object[] { new List<int>(), 2, 10, new PaginatedResult<int>([], 2, 0) };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }


        [Theory]
        [ClassData(typeof(CorrectPaginationTestData))]
        public void GetResult_CorrectData_CorrectResult(IEnumerable<int> data, int page, int size, PaginatedResult<int> expectedResult)
        {
            Assert.Equal(pagination.GetResult(data, page, size), expectedResult);
        }

        public class WrongPaginationTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { inputData, 0, 10 };
                yield return new object[] { inputData, -1, 10 };
                yield return new object[] { inputData, 1, 0 };
                yield return new object[] { inputData, 1, -1 };
                
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(WrongPaginationTestData))]
        public void GetResult_WrongInputData_ThrowException(IEnumerable<int> data, int page, int size)
        {
            Assert.Throws<RequestWrongParameterException>(() => pagination.GetResult(data, page, size));
        }

        
    }

    
}
using BestEvents;

namespace BestEventsTest
{
    public class PaginationTest
    {
        


        static List<int> inputData = [.. Enumerable.Range(1, 98)];


        static Pagination<int> IntPagination = new Pagination<int>();

        [Fact]
        public void GetResult_CorrectInputData_CorrectResult()
        {


        }

        [Fact]
        public void GetResult_WrongInputData_ThrowException()
        {


        }

        
    }

    
}
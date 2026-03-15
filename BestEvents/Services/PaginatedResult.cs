namespace BestEvents
{
    /// <summary>
    /// Результаты поиска в репозитории c фильтрацией и пагинацией 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultsPerPage"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalResultsNumber"></param>
        public PaginatedResult(List<T> resultsPerPage, int currentPage, int totalResultsNumber)
        {
            ResultsPerPage = resultsPerPage;
            CurrentPage = currentPage;
            TotalResultsNumber = totalResultsNumber;
            ResultsNumberPerPage = resultsPerPage.Count();
        }

        /// <summary>
        /// Общее количество записей
        /// </summary>
        public int TotalResultsNumber { get; }

        /// <summary>
        /// Содержание
        /// </summary>
        public List<T> ResultsPerPage { get; }

        /// <summary>
        /// Текущая страница
        /// </summary>
        public int CurrentPage { get; }

        /// <summary>
        /// Количество записей на странице
        /// </summary>
        public int ResultsNumberPerPage { get; }
    }
}

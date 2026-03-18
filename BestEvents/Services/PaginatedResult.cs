namespace BestEvents
{
    /// <summary>
    /// Результаты поиска в репозитории c фильтрацией и пагинацией 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// /// <param name="resultsPerPage"></param>
    /// <param name="currentPage"></param>
    /// /// <param name="resultsNumberPerPage"></param>
    /// <param name="totalResultsNumber"></param>

    public record PaginatedResult<T>(List<T> resultsPerPage, int currentPage, int resultsNumberPerPage, int totalResultsNumber)
    {
        /// <summary>
        /// Общее количество записей
        /// </summary>
        public int TotalResultsNumber { get; } = totalResultsNumber;

        /// <summary>
        /// Содержание
        /// </summary>
        public List<T> ResultsPerPage { get; } = resultsPerPage;

        /// <summary>
        /// Текущая страница
        /// </summary>
        public int CurrentPage { get; } = currentPage;

        /// <summary>
        /// Количество записей на странице
        /// </summary>
        public int ResultsNumberPerPage { get; } = resultsNumberPerPage;

    }
}

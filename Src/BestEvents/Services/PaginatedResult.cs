namespace BestEvents
{
    /// <summary>
    /// Результаты поиска в репозитории c фильтрацией и пагинацией 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resultsOnPage"></param>
    /// <param name="currentPage"></param>
    /// <param name="totalResultsNumber"></param>

    public class PaginatedResult<T>(IEnumerable<T> resultsOnPage, int currentPage, int totalResultsNumber): IEquatable<PaginatedResult<T>>
    {
        /// <summary>
        /// Общее количество записей
        /// </summary>
        public int TotalResultsNumber { get; } = totalResultsNumber;

        /// <summary>
        /// Содержание
        /// </summary>
        public IEnumerable<T> ResultsOnPage { get; } = resultsOnPage;

        /// <summary>
        /// Текущая страница
        /// </summary>
        public int CurrentPage { get; } = currentPage;

        /// <summary>
        /// Количество записей на странице
        /// </summary>
        public int ResultsNumberOnPage { get; } = resultsOnPage.Count();

        

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not PaginatedResult<T> _other)
                return false;
            return Equals(_other);
        }

        /// <inheritdoc/>
        public bool Equals(PaginatedResult<T>? other)
        {
            if (other == null)
                return false;
            if (TotalResultsNumber != other.TotalResultsNumber)
                return false;
            if (CurrentPage != other.CurrentPage)
                return false;
            if (ResultsNumberOnPage != other.ResultsNumberOnPage)
                return false;
            if (ResultsOnPage.Count() != other.ResultsOnPage.Count())
                return false;
            if (!ResultsOnPage.SequenceEqual(other.ResultsOnPage))
                return false;
            return true;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(TotalResultsNumber, CurrentPage, ResultsNumberOnPage, ResultsOnPage);

        }
    }
}

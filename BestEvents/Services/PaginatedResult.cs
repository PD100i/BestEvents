namespace BestEvents
{
    /// <summary>
    /// Результаты поиска в репозитории c фильтрацией и пагинацией 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// /// <param name="resultsPerPage"></param>
    /// <param name="currentPage"></param>
    /// <param name="totalResultsNumber"></param>

    public class PaginatedResult<T>(List<T> resultsPerPage, int currentPage, int totalResultsNumber): IEquatable<PaginatedResult<T>>
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
        public int ResultsNumberPerPage { get; } = resultsPerPage.Count;

        

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
            if (ResultsNumberPerPage != other.ResultsNumberPerPage)
                return false;
            if (ResultsPerPage.Count != other.ResultsPerPage.Count)
                return false;
            if (!ResultsPerPage.SequenceEqual(other.ResultsPerPage))
                return false;
            return true;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(TotalResultsNumber, CurrentPage, ResultsNumberPerPage, ResultsPerPage);

        }
    }
}

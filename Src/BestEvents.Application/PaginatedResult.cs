namespace BestEvents.Application
{

    /// <summary>
    /// Класс для представления результатов поиска в репозитории с фильтрацией и пагинацией.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedResult<T> : IEquatable<PaginatedResult<T>>
    {
        /// <summary>
        /// Конструктор для инициализации пустого результата с пагинацией.
        /// </summary>
        public PaginatedResult()
        {
        }


        /// <summary>
        /// Результаты поиска в репозитории c фильтрацией и пагинацией 
        /// </summary>
        /// <param name="resultsOnPage"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalResultsNumber"></param>
        public PaginatedResult (IEnumerable<T> resultsOnPage, int currentPage, int totalResultsNumber)
        {
            ResultsOnPage = resultsOnPage;
            CurrentPage = currentPage;
            TotalResultsNumber = totalResultsNumber;
            ResultsNumberOnPage = resultsOnPage.Count();
        }

        /// <summary>
        /// Общее количество записей
        /// </summary>
        public int TotalResultsNumber { get; set; }

        /// <summary>
        /// Содержание
        /// </summary>
        public IEnumerable<T> ResultsOnPage { get; set; } = Array.Empty<T>();

        /// <summary>
        /// Текущая страница
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Количество записей на странице
        /// </summary>
        public int ResultsNumberOnPage { get; set; } 

        

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

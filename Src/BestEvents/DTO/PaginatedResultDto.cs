using System.Linq;

namespace BestEvents
{
    /// <summary>
    /// DTO для возврата результатов пагинации
    /// </summary>
    public class PaginatedResultDto: IEquatable<PaginatedResultDto>
    {
        /// <summary>
        /// Общее количество событий
        /// </summary>
        public required int TotalResultsNumber { get; set; }

        /// <summary>
        /// События
        /// </summary>
        public required List<EventInfo> ResultsOnPage { get; set; } = [];

        /// <summary>
        /// Текущая страница
        /// </summary>
        public required int CurrentPage { get; set; }

        /// <summary>
        /// Количество событий на странице
        /// </summary>
        public required int ResultsNumberOnPage { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not PaginatedResultDto _other)
                return false;
            return Equals(_other);
        }

        /// <inheritdoc/>
        public bool Equals(PaginatedResultDto? other)
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

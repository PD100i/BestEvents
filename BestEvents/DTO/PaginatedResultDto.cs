namespace BestEvents
{
    /// <summary>
    /// DTO для возврата результатов пагинации
    /// </summary>
    public class PaginatedResultDto
    {
        /// <summary>
        /// Общее количество событий
        /// </summary>
        public required int EventsNumber { get; set; }

        /// <summary>
        /// События
        /// </summary>
        public required List<EventDto> Events { get; set; } = [];

        /// <summary>
        /// Текущая страница
        /// </summary>
        public required int CurrentPage { get; set; }

        /// <summary>
        /// Количество событий на странице
        /// </summary>
        public required int EventsNumberPerPage { get; set; }
    }
}

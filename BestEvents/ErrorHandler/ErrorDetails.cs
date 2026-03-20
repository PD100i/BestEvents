namespace BestEvents.ErrorHandler
{
    /// <summary>
    /// Класс детализации ошибок (по RFC7807)
    /// </summary>
    public class ErrorDetails
    {
        /// <summary>
        /// URI, описывающий тип ошибки
        /// </summary>
        public string Type { get; set; } = "about:blank";

        /// <summary>
        /// Краткое описание ошибки
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// Код статуса
        /// </summary>
        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// Детальное описание ошибки
        /// </summary>
        public string Detail { get; set; } = "";

        /// <summary>
        /// URI, указывающий на запрос, вызвавший ошибку
        /// </summary>
        public string Instance { get; set; } = "";
    }
}

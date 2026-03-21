namespace BestEvents
{
    /// <summary>
    /// Метод расширения для добавления ErrorHandlerMiddleware в конвейер обработки HTTP запросов
    /// </summary>
    public static class ErrorHandlerMiddlewareExtended
    {
        /// <summary>
        /// Добавляет ErrorHandlerMiddleware в конвейер обработки HTTP запросов
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}

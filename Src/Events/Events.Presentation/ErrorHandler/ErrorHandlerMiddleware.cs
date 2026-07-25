
using Events.Domain.Exceptions;
using Events.Application.Exceptions;




namespace Events.Presentation
{
    /// <summary>
    /// Обработчик ошибок для HTTP запросов. Перехватывает исключения, возникающие при обработке запросов, и возвращает соответствующие HTTP статусы и сообщения об ошибках.
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        /// <summary>
        /// Конструктор обработчика исключений
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Стандартный метод для обработки HTTP запросов с обработчиком исключений
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch (EventNotFoundException ex)
            {
                _logger.LogInformation(ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 404;
                ErrorDetails details = new()
                {
                    Title = Messages_ru.EventNotFound,
                    StatusCode = context.Response.StatusCode,
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsJsonAsync(details);
            }
            catch (EventNotExistsException ex)
            {
                _logger.LogInformation(ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 404;
                ErrorDetails details = new()
                {
                    Title = "Бронирование не подтверждено",
                    StatusCode = context.Response.StatusCode,
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsJsonAsync(details);
            }

            catch (EventWrongParameterException ex)
            {
                _logger.LogInformation(ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;
                ErrorDetails details = new()
                {
                    Title = "Недопустимые параметры в запросе",
                    StatusCode = context.Response.StatusCode,
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsJsonAsync(details);
            }

            
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                ErrorDetails details = new()
                {
                    Title = Messages_ru.UndefinedError,
                    StatusCode = context.Response.StatusCode,
                    Detail = "",
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsJsonAsync(details);
            }
        }
    }
}

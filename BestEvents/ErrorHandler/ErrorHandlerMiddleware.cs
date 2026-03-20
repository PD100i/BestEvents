using BestEvents.ErrorHandler;
using BestEvents.Exceptions;

namespace BestEvents
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
            catch (EventsNotFoundException ex)
            {
                _logger.LogInformation(ex.Message, [context.Request.Path]);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 404;
                ErrorDetails details = new()
                {
                    Title = "Событие не найдено",
                    StatusCode = context.Response.StatusCode,
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsJsonAsync(details);
            }
            catch (RequestWrongParameterException ex)
            {
                _logger.LogInformation(ex.Message, [context.Request.Path]);
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
                _logger.LogError(ex.Message, [context.Request.Path, ex, ex.InnerException]);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                ErrorDetails details = new()
                {
                    Title = "Неизвестный тип ошибки",
                    StatusCode = context.Response.StatusCode,
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsJsonAsync(details);
            }
        }
    }
}

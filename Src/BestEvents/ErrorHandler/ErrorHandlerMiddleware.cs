
using BestEvents.Exceptions;
using System.ComponentModel.DataAnnotations;

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
            catch (EventNotFoundException ex)
            {
                _logger.LogInformation($"Request: {context.Request.Path}. {ex.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 404;
                ErrorDetails details = new()
                {
                    Title = Messages_ru.EventNotFoundShort,
                    StatusCode = context.Response.StatusCode,
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsJsonAsync(details);
            }
            catch (BookingNotFoundException ex)
            {
                _logger.LogInformation($"Request: {context.Request.Path}. {ex.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 404;
                ErrorDetails details = new()
                {
                    Title = Messages_ru.BookingNotFoundShort,
                    StatusCode = context.Response.StatusCode,
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsJsonAsync(details);
            }
            catch (EventWrongParameterException ex)
            {
                _logger.LogInformation($"Request: {context.Request.Path}. {ex.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;
                ErrorDetails details = new()
                {
                    Title = Messages_ru.RequestWrongParameters,
                    StatusCode = context.Response.StatusCode,
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsJsonAsync(details);
            }

            catch (EventCompletedException ex)
            {
                _logger.LogWarning($"Request: {context.Request.Path}. {ex.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 409;
                ErrorDetails details = new()
                {
                    Title = Messages_ru.BookingNotConfirmed,
                    StatusCode = context.Response.StatusCode,
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsJsonAsync(details);
            }

            catch (NoAvailableSeatsException ex)
            {
                _logger.LogWarning($"Request: {context.Request.Path}. {ex.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 409;
                ErrorDetails details = new()
                {
                    Title = Messages_ru.BookingNotConfirmed,
                    StatusCode = context.Response.StatusCode,
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsJsonAsync(details);
            }

            catch (BookingWrongParameterException ex)
            {
                _logger.LogInformation($"Request: {context.Request.Path}. {ex.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;
                ErrorDetails details = new()
                {
                    Title = Messages_ru.RequestWrongParameters,
                    StatusCode = context.Response.StatusCode,
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsJsonAsync(details);
            }
           
            catch ( Exception ex)
            {
                _logger.LogError(ex, $"Request: {context.Request.Path}. {ex.Message}");
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

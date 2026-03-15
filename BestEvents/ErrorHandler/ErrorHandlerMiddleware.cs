using BestEvents.Exceptions;

namespace BestEvents
{
    /// <summary>
    /// Обработчик ошибок для HTTP запросов. Перехватывает исключения, возникающие при обработке запросов, и возвращает соответствующие HTTP статусы и сообщения об ошибках.
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Конструктор обработчика исключений
        /// </summary>
        /// <param name="next"></param>
        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
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
            catch(EventsNotFoundException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(ex.Message);
            }
            catch(EventWrongParameterException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(ex.Message);
            }
        }
    }
}

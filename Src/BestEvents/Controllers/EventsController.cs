
using BestEvents.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace BestEvents.Controllers
{
    /// <summary>
    /// Контроллер событий
    /// </summary>
    /// <param name="eventService"></param>
    /// <param name="bookingService"></param>
    [ApiController]
    [Route("events")]
    public class EventsController(IEventService eventService, IBookingService bookingService) : ControllerBase
    {

        /// <summary>
        /// Возвращает события, фильтруя их по параметрам name, from, to. Возвращает HTTP статус-код 200 Ok в случае успеха
        /// </summary>
        /// <param name="title">Название события для поиска (необязательный)</param>
        /// <param name="from">Дата для поиска событий, которые начинаются не раньше этой даты (необязательный) </param>
        /// <param name="to">Дата для поиска событий, которые заканчиваются не позже этой даты</param>
        /// <param name="page">Номер страницы для вывода</param>
        /// <param name="pageSize">Количество элементов на странице</param>
        /// <param name="ct">Токен отмены</param>
        /// <response code="200">Возвращается JSON-структура PaginationResultsDto с деталями ответа и HTTP статус-кодом 200 Ok в случае успеха</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResultDto))]
        public async Task<IActionResult> GetEventsAsync([FromQuery] string? title, DateTime? from, DateTime? to, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var result = await eventService.GetEventsAsync(title, from, to, page, pageSize, ct);
            return Ok(result);
        }

        /// <summary>
        /// Возвращает событие по его идентификатору
        /// </summary>
        /// <param name="id">Идентификационный номер для получения события </param>
        /// <param name="ct">Токен отмены</param>
        /// <response code="200">Если событие найдено, возвращается JSON-структура EventDto с деталями ответа</response>
        /// <response code="400">Если id некорректен</response>
        /// <response code="404">Если событие с таким идентификатором не найдено</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventInfoDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetEventAsync([FromRoute] string id, CancellationToken ct = default)
        {
            var result = await eventService.GetEventAsync(id, ct);
            return Ok(result);
        }

        /// <summary>
        /// Создает новое событие и добавляет в репозиторий. Возвращает HTTP статус-код 201 Created в случае успеха
        /// </summary>
        /// <param name="eventDto">JSON структура с параметрами события</param>
        /// <param name="ct">Токен отмены</param>
        /// <response code="201">В случае успешного создания события</response>
        /// <response code="400">Если параметры некорректны</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EventInfoDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> CreateEventAsync([FromBody] CreateEventDto eventDto, CancellationToken ct = default)
        {
            var result = await eventService.CreateEventAsync(eventDto, ct);
            return Created(Request.Path.ToString(), result);
        }


        /// <summary>
        /// Перезаписывает событие с идентификатором id
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <param name="eventDto"></param>
        /// <param name="ct">Токен отмены</param>
        /// <response code="204">В случае успешной записи</response>
        /// <response code="400">Если параметры некорректны</response>
        /// <response code="404">Если событие с таким идентификатором не найдено</response>
        [HttpPut("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> ReplaceEventAsync([FromRoute] string id, [FromBody] EventInfoDto eventDto, CancellationToken ct = default)
        {
            await eventService.ReplaceEventAsync(id, eventDto, ct);
            return NoContent();
        }

        /// <summary>
        /// Удаляет событие с идентификатором id
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <param name="ct">Токен отмены</param>
        /// <response code="204">В случае успешного удаления</response>
        /// <response code="400">Если id некорректен</response>
        /// <response code="404">Если событие с таким идентификатором не найдено</response>
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> DeleteEventAsync([FromRoute] string id, CancellationToken ct = default)
        {
            await eventService.DeleteEventAsync(id, ct);
            return NoContent();
        }

        /// <summary>
        /// Создает бронирование на событие с идентификатором id
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <param name="ct">Токен отмены</param>
        /// <response code="202">В случае успешного бронирования возвращает URL для получения статуса бронирования</response>
        /// <response code="400">Если id некорректен</response>
        /// <response code="404">Если событие с таким идентификатором не найдено</response>
        /// <response code="409">В случае отклонения бронирования, например, если нет свободных мест или событие уже завершилось</response>
        [HttpPost("{id}/book")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> CreateBookingAsync([FromRoute] string id, CancellationToken ct = default)
        {
            var booking = await bookingService.CreateBookingAsync(id, ct);
            return AcceptedAtRoute("GetBookingId", new { id = booking.Id });
        }
    }
}

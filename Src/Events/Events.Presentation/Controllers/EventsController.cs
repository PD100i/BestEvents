

using Events.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Events.Presentation.Controllers
{
    /// <summary>
    /// Контроллер событий
    /// </summary>
    /// <param name="eventService"></param>
    /// <param name="mapper"></param>
    [ApiController]
    [Authorize]
    [Route("events")]
    public class EventsController(IEventService eventService, DtoMapper mapper) : ControllerBase
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
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResultDto))]
        public async Task<IActionResult> GetEventsAsync([FromQuery] string? title, DateTime? from, DateTime? to, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var result = await eventService.GetEventsAsync(title, from, to, page, pageSize, ct);
            return Ok(result);
        }


        /// <summary>
        /// Возвращает топ 10 популярных событий. Возвращает HTTP статус-код 200 Ok в случае успеха
        /// </summary>
        /// <param name="ct">Токен отмены</param>
        /// <response code="200">Возвращается JSON-структура PaginationResultsDto с деталями ответа и HTTP статус-кодом 200 Ok в случае успеха</response>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EventInfoDto>))]
        public async Task<IActionResult> GetTopEventsAsync(CancellationToken ct = default)
        {
            var result = await eventService.GetTopPopularEventsAsync(ct);
            return Ok(mapper.MapEventListToEventInfoDtoList(result));
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
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventInfoDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetEventAsync([FromRoute] string id, CancellationToken ct = default)
        {
            var result = await eventService.GetEventAsync(mapper.StringToGuid(id), ct);
            return Ok(result);
        }

        /// <summary>
        /// Создает новое событие и добавляет в репозиторий. Возвращает HTTP статус-код 201 Created в случае успеха
        /// </summary>
        /// <param name="eventDto">JSON структура с параметрами события</param>
        /// <param name="ct">Токен отмены</param>
        /// <response code="201">В случае успешного создания события</response>
        /// <response code="400">Если параметры некорректны</response>
        /// <response code="401">Если пользователь не авторизован</response>
        /// <response code="403">Если у пользователя нет прав на операцию</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EventInfoDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorDetails))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEventAsync([FromBody] CreateEventDto eventDto, CancellationToken ct = default)
        {
            var result = await eventService.CreateEventAsync(mapper.MapCreateEventDtoToEvent(eventDto), ct);
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
        /// <response code="401">Если пользователь не авторизован</response>
        /// <response code="403">Если у пользователя нет прав на операцию</response>
        /// <response code="404">Если событие с таким идентификатором не найдено</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> ReplaceEventAsync([FromRoute] string id, [FromBody] EventInfoDto eventDto, CancellationToken ct = default)
        {
            await eventService.ReplaceEventAsync(mapper.StringToGuid(id), mapper.MapEventInfoDtoToEvent(eventDto), ct);
            return NoContent();
        }

        /// <summary>
        /// Удаляет событие с идентификатором id
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <param name="ct">Токен отмены</param>
        /// <response code="204">В случае успешного удаления</response>
        /// <response code="400">Если id некорректен</response>
        /// <response code="401">Если пользователь не авторизован</response>
        /// <response code="403">Если у пользователя нет прав на операцию</response>
        /// <response code="404">Если событие с таким идентификатором не найдено</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> DeleteEventAsync([FromRoute] string id, CancellationToken ct = default)
        {
            await eventService.DeleteEventAsync(mapper.StringToGuid(id), ct);
            return NoContent();
        }

        
    }
}

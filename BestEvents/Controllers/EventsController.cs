using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BestEvents.Controllers
{
    /// <summary>
    /// Контроллер событий
    /// </summary>
    /// <param name="eventService"></param>
    [ApiController]
    [Route("api/events")]
    public class EventsController(IEventService eventService) : ControllerBase
    {
        /// <summary>
        /// Возвращает все события. Возвращает HTTP статус-код 200 Ok в случае успеха
        /// </summary>
        /// <response code="200">Возвращается JSON-структура EventDto[] с деталями ответа
        /// и HTTP статус-кодом 200 Ok в случае успеха</response>

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(eventService.GetEvents());
        }

        /// <summary>
        /// Возвращает событие по его идентификатору. Возвращает HTTP статус-код 200 Ok в случае успеха, 
        /// или 404 (Not Found), если событие с таким идентификатором не найдено
        /// </summary>
        /// <param name="id">Идентификационный номер для получения события </param>
        /// <response code="200">Возвращается JSON-структура EventDto с деталями ответа
        /// и HTTP статус-кодом 200 Ok в случае успеха</response>
        [HttpGet("{id}")]
        public IActionResult GetEvent([FromRoute] string id)
        {
            return Ok(eventService.GetEvent(id));
        }

        /// <summary>
        /// Создает новое событие и добавляет в репозиторий. Возвращает HTTP статус-код 201 Created в случае успеха
        /// </summary>
        /// <param name="eventDto">JSON структура с параметрами события</param>
        /// <response code="201"></response>
        [HttpPost]
        public IActionResult CreateEvent([FromBody] EventDtoBase eventDto)
        {
            return Created(Request.Path.ToString(), eventService.CreateEvent(eventDto));
        }


        /// <summary>
        /// Перезаписывает событие с идентификатором id. Возвращает HTTP статус-код 204 No Content в случае успеха, или 404 (Not Found), 
        /// если событие с таким идентификатором не найдено
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <param name="eventDto"></param>
        /// <response code="204"></response>
        [HttpPut("{id}")]
        public IActionResult ReplaceEvent([FromRoute] string id, [FromBody] EventDto eventDto)
        {
            eventService.ReplaceEvent(id, eventDto);
            return NoContent();
        }

        /// <summary>
        /// Удаляет событие с идентификатором id. Возвращает HTTP статус-код 204 No Content  в случае успеха, или 404 (Not Found), 
        /// если событие с таким идентификатором не найдено
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <response code="204"></response>
        [HttpDelete("{id}")]
        public IActionResult DeleteEvent([FromRoute] string id)
        {
            eventService.DeleteEvent(id);
            return NoContent();
        }

        
    }
}

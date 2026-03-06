using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BestEvents.Controllers
{
    /// <summary>
    /// Контроллер событий
    /// </summary>
    /// <param name="eventService"></param>
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController(IEventService eventService) : ControllerBase
    {
        /// <summary>
        /// Возвращает все события
        /// </summary>
        /// <response code="200">Возвращается JSON-структура EventDto[] с деталями ответа
        /// и HTTP статус-кодом 200 Ok в случае успеха</response>

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(eventService.GetEvents());
        }

        /// <summary>
        /// Возвращает событие по его идентификатору
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
        /// Создает новое событие и добавляет в хранилище
        /// </summary>
        /// <param name="eventDto">JSON структура с параметрами события</param>
        /// <response code="201"></response>
        [HttpPost]
        public IActionResult CreateEvent([FromBody] EventDtoBase eventDto)
        {
            eventService.CreateEvent(eventDto);
            return Created();
        }


        /// <summary>
        /// Перезаписывает событие с идентификатором id
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
        /// Удаляет событие с идентификатором id
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

using BestEvents.BookingService;
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
        /// <response code="200">Возвращается JSON-структура EventDto[] с деталями ответа
        /// и HTTP статус-кодом 200 Ok в случае успеха</response>
        [HttpGet]
        public IActionResult GetEvents([FromQuery] string? title, DateTime? from, DateTime? to, int page = 1, int pageSize = 10)
        {
            return Ok(eventService.GetEvents(title, from, to, page, pageSize));
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

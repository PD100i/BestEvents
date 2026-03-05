using Microsoft.AspNetCore.Mvc;

namespace BestEvents.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_eventService.GetEvents());
        }

        [HttpGet]
        public IActionResult GetById([FromRoute]int id)
        {
            return Ok(_eventService.GetEvent(id));
        }

        [HttpPost]
        public IActionResult CreateEvent([FromBody] string title, string descriptor, DateTime startAt, DateTime endAt)
        {
            _eventService.AddEvent()
        }
    }
}

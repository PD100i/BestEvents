using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BestEvents.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController(IEventService eventService) : ControllerBase
    {

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(eventService.GetEvents());
        }

        [HttpGet]
        public IActionResult GetEvent([FromRoute]string id)
        {
            return Ok(eventService.GetEvent(id));
        }

        //[HttpPost]
        //public IActionResult CreateEvent([FromBody] string name, DateTime startAt, DateTime endAt, string description)
        //{
        //    _eventService.CreateEvent(name, startAt, endAt, description);
        //    return Created();
        //}

        [HttpPost]
        public IActionResult CreateEvent([FromBody] EventDto eventDto)
        {
            eventService.CreateEvent(eventDto);
            return Created();
        }

        [HttpPut]
        public IActionResult ReplaceEvent([FromBody] EventDtoExtended eventDto)
        {
            eventService.ReplaceEvent(eventDto);
            return NoContent();
        }

        [HttpDelete]
        public IActionResult DeleteEvent([FromRoute] string id)
        {
            eventService.DeleteEvent(id);
            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BestEvents.Controllers
{
    /// <summary>
    /// Контроллкер бронирований
    /// </summary>
    [Route("booking")]
    public class BookingController(IBookingService bookingService) : ControllerBase
    {


        /// <summary>
        /// Возвращает статус бронирования по его идентификатору. Возвращает HTTP статус-код 200 Ok в случае успеха,         
        /// или 404 (Not Found), если бронирование с таким идентификатором не найдено
        /// </summary>
        /// <param name="id">Идентификационный номер для получения информации о бронировании </param>
        /// <param name="ct">Токен отмены</param>
        /// <response code="200">Если событие найдено, возвращается JSON-структура EventDto с деталями ответа</response>
        /// <response code="400">Если id некорректен</response>
        /// <response code="404">Если бронирования с таким идентификатором не найдено</response>
        [HttpGet("{id}", Name = "GetBookingId")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingResultDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetBookingByIdAync(string id, CancellationToken ct = default)
        {
            return Ok(await bookingService.GetBookingByIdAsync(id, ct));
        }

        
    }
}

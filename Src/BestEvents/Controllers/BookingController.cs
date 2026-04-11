using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BestEvents.Controllers
{
    /// <summary>
    /// Контроллкер бронирований
    /// </summary>
    [ApiController]
    [Route("booking")]
    public class BookingController(IBookingService bookingService) : ControllerBase
    {


        /// <summary>
        /// Возвращает статус бронирования по его идентификатору. Возвращает HTTP статус-код 200 Ok в случае успеха,         
        /// или 404 (Not Found), если бронирование с таким идентификатором не найдено
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetBookingId")]
        public async Task<IActionResult> GetBookingByIdAync(string id, CancellationToken ct = default)
        {
            return Ok(await bookingService.GetBookingByIdAsync(id, ct));
        }

        
    }
}

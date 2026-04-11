using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BestEvents.Controllers
{
    /// <summary>
    /// Контроллкер бронирований
    /// </summary>
    public class BookingController(IBookingService bookingService) : ControllerBase
    {
        /// <summary>
        /// Создает бронирование на событие с идентификатором id. Возвращает HTTP статус-код 202 (Accepted) в случае успеха, 
        /// или 404 (Not Found), если событие с таким идентификатором не найдено
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("events/{id}/book")]
        public async Task<IActionResult> CreateBookingAsync([FromRoute] string id, CancellationToken ct)
        {
            var booking = await bookingService.CreateBookingAsync(id, ct);
            return Accepted($"booking/{booking.Id}");
        }

        /// <summary>
        /// Возвращает статус бронирования по его идентификатору. Возвращает HTTP статус-код 200 Ok в случае успеха,         
        /// или 404 (Not Found), если бронирование с таким идентификатором не найдено
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("booking/{id}")]
        public async Task<IActionResult> GetBookingByIdAsync([FromRoute] string id, CancellationToken ct)
        {
            return Ok(await bookingService.GetBookingByIdAsync(id, ct));
        }
    }
}

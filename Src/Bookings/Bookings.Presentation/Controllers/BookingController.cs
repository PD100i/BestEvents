using Bookings.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Bookings.Presentation.Controllers
{
    /// <summary>
    /// Контроллкер бронирований
    /// </summary>
    [Route("booking")]
    [Authorize]
    public class BookingController(IBookingService bookingService, DtoMapper mapper) : ControllerBase
    {
        /// <summary>
        /// Создает бронирование на событие с идентификатором id
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <param name="ct">Токен отмены</param>
        /// <response code="202">В случае успешного бронирования возвращает URL для получения статуса бронирования</response>
        /// <response code="400">Если id некорректен, событие началось или завершилось</response>
        /// <response code="401">Если пользователь не авторизован</response>
        /// <response code="403">Если у пользователя нет прав на операцию</response>
        /// <response code="404">Если событие с таким идентификатором не найдено</response>
        /// <response code="409">В случае отклонения бронирования, например, если нет свободных мест или лимит бронирования для пользователя исчерпан</response>
        [HttpPost("{id}/book")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> CreateBookingAsync([FromRoute] string id, CancellationToken ct = default)
        {
            var booking = await bookingService.CreateBookingAsync(mapper.StringToGuid(id), ct);
            return AcceptedAtRoute("GetBookingId", new { id = booking.Id });
        }


        /// <summary>
        /// Возвращает статус бронирования по его идентификатору. Возвращает HTTP статус-код 200 Ok в случае успеха,         
        /// или 404 (Not Found), если бронирование с таким идентификатором не найдено
        /// </summary>
        /// <param name="id">Идентификационный номер для получения информации о бронировании </param>
        /// <param name="ct">Токен отмены</param>
        /// <response code="200">Если событие найдено, возвращается JSON-структура EventDto с деталями ответа</response>
        /// <response code="400">Если id некорректен</response>
        /// <response code="401">Если пользователь не авторизован</response>
        /// <response code="404">Если бронирования с таким идентификатором не найдено</response>
        [HttpGet("{id}", Name = "GetBookingId")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingResultDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetBookingByIdAync(string id, CancellationToken ct = default)
        {
            return Ok(mapper.MapBookingToBookingResultDto(await bookingService.GetBookingAsync(mapper.StringToGuid(id), ct)));
        }

        /// <summary>
        /// Отменяет бронирование по его идентификатору. Возвращает HTTP статус-код 204 No Content в случае успеха,
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <response code="204">Если бронирование успешно отменено</response>
        /// <response code="401">Если пользователь не авторизован</response>
        /// <response code="403">Если пользователь не имеет прав на отмену бронирования</response>
        /// <response code="404">Если бронирования с таким идентификатором не найдено</response>
        [HttpDelete("{id}", Name = "CancelBookingId")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> CancelBookingAsync(string id, CancellationToken ct = default)
        {
            await bookingService.CancelBookingAsync(mapper.StringToGuid(id), ct);
            return NoContent();
        }
    }
}
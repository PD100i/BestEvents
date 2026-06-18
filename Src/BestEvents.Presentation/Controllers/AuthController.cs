using BestEvents.Application;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BestEvents.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для аутентификации и авторизации пользователей
    /// </summary>
    /// <param name="identityService"></param>
    [ApiController]
    [Route("auth")]
    public class AuthController(IUserIdentityService identityService) : ControllerBase
    {
        /// <summary>
        /// Регистрирует нового пользователя
        /// </summary>
        /// <param name="model">DTO регистрации пользователя</param>
        /// <returns></returns>
        [HttpPost("register")]
        /// <param name="ct">Токен отмены</param>
        /// <response code="200">Если событие найдено, возвращается JSON-структура EventDto с деталями ответа</response>
        /// <response code="400">Если id некорректен</response>
        /// <response code="404">Если бронирования с таким идентификатором не найдено</response>
        public async Task<IActionResult> Register([FromBody] UserRegisterDto model, CancellationToken ct = default)
        {
            await identityService.RegistrUserAsync(model.UserName, model.Password, model.Role, ct);
            return NoContent();
        }
    }
}

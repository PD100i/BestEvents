using BestEvents.Application;
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
        /// <param name="ct">Токен отмены</param>
        /// <response code="204">Если пользователь успешно зарегистрирован</response>
        /// <response code="400">Если данные пользователя некорректны</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto model, CancellationToken ct = default)
        {
            await identityService.RegistrUserAsync(model.UserName, model.Password, model.Role, ct);
            return NoContent();
        }

        /// <summary>
        /// Аутентифицирует пользователя и возвращает JWT-токен для доступа к защищенным ресурсам
        /// </summary>
        /// <param name="model">DTO для входа пользователя</param>
        /// <param name="ct">Токен отмены</param>
        /// <response code="200">JWT-токен для доступа к защищенным ресурсам</response>
        /// <response code="404">Если пользователь не найден</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model, CancellationToken ct = default)
        {
            var token = await identityService.GetTokenAsync(model.UserName, model.Password, ct);
            return Ok(new { Token = token });
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace BestEvents.Presentation
{
    /// <summary>
    /// DTO для регистрации пользователя
    /// </summary>
    public class UserRegisterDto
    {
        /// <summary>
        /// Логин
        /// </summary>
        [Required]
        public string UserName { get; set; } = "";

        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = "";

        /// <summary>
        /// Роль. Принимает значения User и Admin. По умолчанию будет USer
        /// </summary>
        public string? Role {  get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Users.Presentation
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
        [StringLength(20, MinimumLength = 4)]
        public string UserName { get; set; } = "";

        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; } = "";

        /// <summary>
        /// Роль. Принимает значения User и Admin. По умолчанию будет USer
        /// </summary>
        public string? Role {  get; set; }
    }
}

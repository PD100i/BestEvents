using System.ComponentModel.DataAnnotations;

namespace BestEvents.Presentation
{
    /// <summary>
    /// DTO для входа пользователя в систему
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Имя пользователя
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEvents.Domain
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Хеш пароля
        /// </summary>
        public string PasswordHash { get; set; } = "";

        /// <summary>
        /// Роль
        /// </summary>
        public RolesEnum Role { get; set; }

        public List<Booking> Bookings { get; set; } = [];
    }

    /// <summary>
    /// Перечисление ролей пользователя
    /// </summary>
    public enum RolesEnum
    {
        User,
        Admin
    }
}

using BestEvents.Domain.Exceptions;
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
        public User() { }

        public User(Guid userId, string name, string passwordHash, string? role) 
        {
            if (userId == default)
                throw new UserRegisterException(Messages_ru.WrongUserId);

            if (string.IsNullOrEmpty(name))
                throw new UserRegisterException(Messages_ru.UserNameIsEmpty);

            if (! (name.All(c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))))
                throw new UserRegisterException(Messages_ru.WrongUserNameFormat);

            if (string.IsNullOrEmpty(role) || role == "User")
                Role = UserRolesEnum.User;
            else if (role == "Admin")
                Role = UserRolesEnum.Admin;
            else
                throw new UserRegisterException(string.Format(Messages_ru.WrongRole, role));

            Id = userId;
            Name = name;
            PasswordHash = passwordHash;
        }

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
        public UserRolesEnum Role { get; set; }

        public List<Booking> Bookings { get; set; } = [];
    }

    /// <summary>
    /// Перечисление ролей пользователя
    /// </summary>
    public enum UserRolesEnum
    {
        User,
        Admin
    }
}

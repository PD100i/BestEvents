using BestEvents.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        public static User CreateUser(Guid userId, string name, string passwordHash, string? role) 
        {
            User user = CreateUser(userId, name, role);
            user.PasswordHash = passwordHash;
            return user;
        }

        public static User CreateUser(string? userId, string? name, string? role)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UserWrongParameterException(Messages_ru.UserIdIsNotSent);
            if (!Guid.TryParse(userId, out Guid id))
                throw new UserWrongParameterException(Messages_ru.WrongUserId);
            return CreateUser(id, name, role);
        }

        private static User CreateUser(Guid userId, string? name, string? role)
        {
            User user = new User();

            if (string.IsNullOrEmpty(name))
                throw new UserWrongParameterException(Messages_ru.UserNameIsNotSent);

            if (string.IsNullOrEmpty(role) || role == "User")
                user.Role = UserRolesEnum.User;
            else if (role == "Admin")
                user.Role = UserRolesEnum.Admin;
            else
                throw new UserWrongParameterException(string.Format(Messages_ru.WrongRole, role));

            user.Id = userId;
            user.Name = name;
            return user;
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

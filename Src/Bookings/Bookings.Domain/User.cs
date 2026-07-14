using Bookings.Domain.Exceptions;

namespace Bookings.Domain
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User
    {
        public User() { }

        public User(string? id, string? name, string? role)
        {
            if (id == null) 
                throw new CreateUserException(string.Format(Messages_ru.WrongUserId, "null"));
            if (!Guid.TryParse(id, out var userId))
                throw new CreateUserException(string.Format(Messages_ru.WrongUserId, id));
            Id = userId;
            if (name == null)
                throw new CreateUserException(Messages_ru.EmptyUserName);
            if (role == "Admin")
                Role = UserRolesEnum.Admin;
            else if (role == "User")
                Role = UserRolesEnum.User;
            else
                throw new CreateUserException(string.Format(Messages_ru.WrongRole, id));
            Name = name;

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
        /// Роль
        /// </summary>
        public UserRolesEnum Role { get; set; }
      
    }

    /// <summary>
    /// Перечисление ролей пользователя
    /// </summary>
    public enum UserRolesEnum
    {
        NotDefined = default,
        User,
        Admin
    }
}

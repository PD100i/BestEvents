using BestEvents.Domain;

namespace BestEvents.Infrastructure
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class UserEntity
    {
        public UserEntity() { }

        
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

        /// <summary>
        /// Список бронирований, сделанных пользователем
        /// </summary>
        public List<BookingEntity> Bookings { get; set; } = [];
    }

   
}

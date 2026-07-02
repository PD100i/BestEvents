using Users.Domain;
using Riok.Mapperly.Abstractions;

namespace Users.Infrastructure
{
    /// <summary>
    /// Класс для маппинга между сущностями EventEntity и BookingEntity и доменными моделями Event и Booking.
    /// </summary>
    [Mapper]
    public partial class EntityMapper
    {
        /// <summary>
        /// Мапинг из доменной модели User в сущность UserEntity для сохранения в базе данных.
        /// </summary>
        /// <param name="user">Доменная модель User, которую нужно преобразовать в сущность.</param>
        public partial UserEntity MapUserToEntity(User user);

        /// <summary>
        /// Мапинг из сущности UserEntity в доменную модель User для использования в бизнес-логике приложения.
        /// </summary>
        /// <param name="entity">Сущность UserEntity, которую нужно преобразовать в доменную модель.</param>
        /// <returns>Доменная модель User, соответствующая сущности UserEntity.</returns>
        public partial User MapEntityToUser(UserEntity entity);

        /// <summary>
        /// Приведение DateTime к UTC для корректного сохранения в базе данных и обеспечения единообразия при работе с датами и временем.
        /// </summary>
        /// <param name="dt">Дата и время, которые нужно привести к UTC.</param>
        /// <returns>Дата и время в формате UTC.</returns>
        public DateTime MapToUtc(DateTime dt)
        {
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        }
    }
}

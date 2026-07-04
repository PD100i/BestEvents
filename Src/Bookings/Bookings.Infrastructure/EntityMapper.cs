using Bookings.Domain;
using Common;
using Riok.Mapperly.Abstractions;

namespace Bookings.Infrastructure
{
    /// <summary>
    /// Класс для маппинга между сущностями EventEntity и BookingEntity и доменными моделями Event и Booking.
    /// </summary>
    [Mapper]
    public partial class EntityMapper
    {    

        /// <summary>
        /// Мапинг из доменной модели Booking в сущность BookingEntity для сохранения в базе данных.
        /// </summary>
        /// <param name="booking">Доменная модель Booking, которую нужно преобразовать в сущность.</param>
        /// <returns>Сущность BookingEntity, соответствующая доменной модели Booking.</returns>
        public partial BookingEntity MapBookingToEntity(Booking booking);

        /// <summary>
        /// Обновление существующей сущности BookingEntity на основе данных из доменной модели Booking.
        /// </summary>
        /// <param name="booking">Доменная модель Booking, содержащая новые данные.</param>
        /// <param name="entity">Сущность BookingEntity, которую нужно обновить.</param>
        public partial void UpdateBookingEntity(Booking booking, BookingEntity entity);

        /// <summary>
        /// Мапинг из сущности BookingEntity в доменную модель Booking для использования в бизнес-логике приложения.
        /// </summary>
        /// <param name="entity">Сущность BookingEntity, которую нужно преобразовать в доменную модель.</param>
        /// <returns>Доменная модель Booking, соответствующая сущности BookingEntity.</returns>        
        public partial Booking MapEntityToBooking(BookingEntity entity);

        /// <summary>
        /// Мапинг из модели сообщения в сущность для сохранения в базе данных
        /// </summary>
        /// <param name="entity">Модель сообщения</param>
        /// <returns></returns>
        public partial BookingMessage MapBookingCreatedMessageToEntity (BookingCreatedMessage entity);

        /// <summary>
        /// Мапинг их сущности базы данных в модель
        /// </summary>
        /// <param name="entity">Сущность базы данных</param>
        /// <returns></returns>
        public partial BookingCreatedMessage MapBookingCreatedEntityToMessage(BookingMessage entity);

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

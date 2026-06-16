using BestEvents.Domain;
using BestEvents.Application;
using Riok.Mapperly.Abstractions;

namespace BestEvents.Infrastructure
{
    /// <summary>
    /// Класс для маппинга между сущностями EventEntity и BookingEntity и доменными моделями Event и Booking.
    /// </summary>
    [Mapper]
    public partial class EntityMapper
    {
        /// <summary>
        /// Мапинг из доменной модели Event в сущность EventEntity для сохранения в базе данных.
        /// </summary>
        /// <param name="_event">Доменная модель Event, которую нужно преобразовать в сущность.</param>
        /// <returns>Сущность EventEntity, соответствующая доменной модели Event.</returns>
        [MapperIgnoreTarget(nameof(EventEntity.Bookings))]
        public partial EventEntity MapEventToEntity(Event _event);

        /// <summary>
        /// Обновление существующей сущности EventEntity на основе данных из доменной модели Event.
        /// </summary>
        /// <param name="_event">Доменная модель Event, содержащая новые данные.</param>
        /// <param name="entity">Сущность EventEntity, которую нужно обновить.</param>
        [MapperIgnoreTarget(nameof(EventEntity.Bookings))]
        public partial void UpdateEventEntity(Event _event, EventEntity entity);

        /// <summary>
        /// Мапинг из сущности EventEntity в доменную модель Event для использования в бизнес-логике приложения.
        /// </summary>
        /// <param name="entity">Сущность EventEntity, которую нужно преобразовать в доменную модель.</param>
        /// <returns>Доменная модель Event, соответствующая сущности EventEntity.</returns>
        [MapperIgnoreSource(nameof(EventEntity.Bookings))]
        public partial Event MapEntityToEvent(EventEntity entity);

        /// <summary>
        /// Обновление существующей доменной модели Event на основе данных из сущности EventEntity.
        /// </summary>
        /// <param name="entity">Сущность EventEntity, содержащая новые данные.</param>
        /// <param name="_event">Доменная модель Event, которую нужно обновить.</param>
        [MapperIgnoreSource(nameof(EventEntity.Bookings))]
        public partial void UpdateEvent(EventEntity entity, Event _event);

        /// <summary>
        /// Мапинг из доменной модели Booking в сущность BookingEntity для сохранения в базе данных.
        /// </summary>
        /// <param name="booking">Доменная модель Booking, которую нужно преобразовать в сущность.</param>
        /// <returns>Сущность BookingEntity, соответствующая доменной модели Booking.</returns>

        [MapperIgnoreSource(nameof(Booking.Event))]
        [MapperIgnoreTarget(nameof(BookingEntity.Event))]
        public partial BookingEntity MapBookingToEntity(Booking booking);

        /// <summary>
        /// Обновление существующей сущности BookingEntity на основе данных из доменной модели Booking.
        /// </summary>
        /// <param name="booking">Доменная модель Booking, содержащая новые данные.</param>
        /// <param name="entity">Сущность BookingEntity, которую нужно обновить.</param>
        /// 
        [MapperIgnoreSource(nameof(Booking.Event))]
        [MapperIgnoreTarget(nameof(BookingEntity.Event))]
        public partial void UpdateBookingEntity(Booking booking, BookingEntity entity);

        /// <summary>
        /// Мапинг из сущности BookingEntity в доменную модель Booking для использования в бизнес-логике приложения.
        /// </summary>
        /// <param name="entity">Сущность BookingEntity, которую нужно преобразовать в доменную модель.</param>
        /// <returns>Доменная модель Booking, соответствующая сущности BookingEntity.</returns>        
        public partial Booking MapEntityToBooking(BookingEntity entity);

        /// <summary>
        /// Маппинг из PaginatedResult/<EventEntity/> в PaginatedResult/<Event/> для использования в бизнес-логике приложения.
        /// </summary>
        /// <param name="paginatedResult">PaginatedResult/<EventEntity/>, который нужно преобразовать.</param>
        /// <returns>PaginatedResult/<Event/>, соответствующий PaginatedResult/<EventEntity/>.</returns>
        public partial PaginatedResult<Event> MapPaginatedResultToEntity(PaginatedResult<EventEntity> paginatedResult);

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

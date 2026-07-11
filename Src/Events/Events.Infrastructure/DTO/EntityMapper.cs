using Common;
using Events.Application;
using Events.Domain;
using Riok.Mapperly.Abstractions;

namespace Events.Infrastructure
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
        public partial EventEntity MapEventToEntity(Event _event);

        /// <summary>
        /// Обновление существующей сущности EventEntity на основе данных из доменной модели Event.
        /// </summary>
        /// <param name="_event">Доменная модель Event, содержащая новые данные.</param>
        /// <param name="entity">Сущность EventEntity, которую нужно обновить.</param>
        public partial void UpdateEventEntity(Event _event, EventEntity entity);

        /// <summary>
        /// Мапинг из сущности EventEntity в доменную модель Event для использования в бизнес-логике приложения.
        /// </summary>
        /// <param name="entity">Сущность EventEntity, которую нужно преобразовать в доменную модель.</param>
        /// <returns>Доменная модель Event, соответствующая сущности EventEntity.</returns>

        public partial Event MapEntityToEvent(EventEntity entity);

        /// <summary>
        /// Обновление существующей доменной модели Event на основе данных из сущности EventEntity.
        /// </summary>
        /// <param name="entity">Сущность EventEntity, содержащая новые данные.</param>
        /// <param name="_event">Доменная модель Event, которую нужно обновить.</param>
        public partial void UpdateEvent(EventEntity entity, Event _event);

                

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

        /// <summary>
        /// Мапинг из модели сообщения в сущность для сохранения в базе данных
        /// </summary>
        /// <param name="entity">Модель сообщения</param>
        /// <returns></returns>
        public partial BookingEventMessageEntity MapBookingMessageToEntity(Message entity);

        /// <summary>
        /// Мапинг их сущности базы данных в модель
        /// </summary>
        /// <param name="entity">Сущность базы данных</param>
        /// <returns></returns>
        public partial Message MapBookingMessageEntityToMessage(BookingEventMessageEntity entity);
    }
}

using Events.Domain;
using Events.Domain.Exceptions;
using Events.Application;
using Riok.Mapperly.Abstractions;


namespace Events.Presentation
{
    /// <summary>
    /// Маппер для преобразования между доменными моделями и DTO
    /// </summary>
    [Mapper]
    public partial class DtoMapper
    {
        /// <summary>
        /// Мапинг из доменной модели Event в DTO EventInfoDto для передачи данных в контроллере и отображения пользователю.
        /// </summary>
        /// <param name="_event">Доменная модель события</param>
        /// <returns>DTO модель события</returns>
        public partial EventInfoDto MapEventToEventInfoDto(Event _event);

        /// <summary>
        /// Мапинг из DTO EventInfoDto в доменную модель Event для обработки данных в сервисе и бизнес-логике.
        /// </summary>
        /// <param name="eventInfoDto">DTO модель события</param>
        /// <returns>Доменная модель события</returns>
        public Event MapEventInfoDtoToEvent(EventInfoDto eventInfoDto)
        {
            return Event.CreateInstanceEvent(StringToGuid(eventInfoDto.Id), eventInfoDto.Title, eventInfoDto.StartAt, eventInfoDto.EndAt,
                eventInfoDto.Description, eventInfoDto.TotalSeats, eventInfoDto.AvailableSeats);
        }

        /// <summary>
        /// Мапинг из DTO CreateEventDto в доменную модель Event для создания нового события на основе данных, полученных от пользователя через контроллер.
        /// </summary>
        /// <param name="createEventDto">DTO модель события для создания</param>
        /// <returns>Доменная модель события</returns>
        public Event MapCreateEventDtoToEvent(CreateEventDto createEventDto)
        {
            return Event.CreateNewEvent(Guid.NewGuid(), createEventDto.Title, createEventDto.StartAt, createEventDto.EndAt, createEventDto.Description, createEventDto.TotalSeats);
        }

        /// <summary>
        /// Мапинг из доменной модели PaginatedResult в PaginatedResultDto
        /// </summary>
        /// <param name="paginatedResult">Доменная модель пагинированного результата</param>
        /// <returns>DTO модель пагинированного результата</returns>
        public partial PaginatedResultDto MapPaginatedResultToPaginationResultDto(PaginatedResult<Event> paginatedResult);

        /// <summary>
        /// Мапинг из списка доменных моделей Event в список DTO EventInfoDto для передачи данных в контроллере и отображения пользователю
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        public partial List<EventInfoDto> MapEventListToEventInfoDtoList(List<Event> events);


        /// <summary>
        /// Мапинг из строки в Guid с проверкой формата. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="EventWrongParameterException"></exception>
        public Guid StringToGuid(string id)
        {
            return Guid.TryParse(id, out Guid result) ? result : throw new EventWrongParameterException(Messages_ru.WrongIdFormat);
        }

        private string GuidToString(Guid id) => id.ToString();

    }
}

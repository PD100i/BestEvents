
using BestEvents.Exceptions;

namespace BestEvents
{
    /// <summary>
    /// Сервис событий, реализующий интерфейс IEventService. 
    /// Использует репозиторий для получения данных и преобразования их 
    /// в Dto объекты для передачи в контроллеры и обратно
    /// </summary>
    public class EventService : IEventService
    {
        readonly IEventRepository repository;

        /// <summary>
        /// Конструктор, принимающий репозиторий для работы с данными.
        /// </summary>
        /// <param name="repository"></param>
        public EventService(IEventRepository repository)
        {
            this.repository = repository;
        }


        /// <summary>
        /// Создает новое событие, используя данные из Dto объекта и передает их в репозиторий для сохранения
        /// </summary>
        /// <param name="_event"></param>
        public EventDto CreateEvent(EventDtoBase _event)
        {
            return GetDtoFromEvent(repository.CreateEvent(_event.Title, _event.StartAt, _event.EndAt, _event.Description));
        }

        /// <summary>
        /// Удаляет событие из репозитория по идентификатору 
        /// </summary>
        /// <param name="id"></param>
        public void DeleteEvent(string id)
        {
            repository.RemoveEvent(ParseStringId(id));
        }

        /// <summary>
        /// Получает событие из репозитория по идентификатору, преобразует его в Dto объект и возвращает.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EventDto GetEvent(string id)
        {
            return GetDtoFromEvent(repository.GetEvent(ParseStringId(id)));
        }

        /// <summary>
        /// Получает все события из репозитория, преобразует их в Dto объекты и возвращает в виде списка.
        /// </summary>
        /// <returns></returns>
        public PaginatedResultDto GetEvents(string? title, DateTime? from, DateTime? to, int page = 1, int size = 10)
        {
            var events = repository.GetEvents(title, from, to, page, size);
            var eventsDto = new List<EventDto>();
            events.ResultsPerPage.ForEach(e => eventsDto.Add(GetDtoFromEvent(e)));

            return new PaginatedResultDto()
            {
                CurrentPage = events.CurrentPage,
                EventsNumber = events.TotalResultsNumber,
                Events = eventsDto,
                EventsNumberPerPage = events.ResultsNumberPerPage
            };
           
        }

        /// <summary>
        /// Перезаписывает событие в репозитории, используя данные из Dto объекта.
        /// </summary>
        /// <param name="eventDto"></param>
        /// /// <param name="id"></param>
        public void ReplaceEvent(string id, EventDto eventDto)
        {
            if(id != eventDto.Id)
                throw new EventWrongParameterException("Параметр id в строке запроса не совпадает с параметром id в теле запроса");
            repository.ReplaceEvent(new Event(ParseStringId(eventDto.Id), eventDto.Title, eventDto.StartAt, eventDto.EndAt, eventDto.Description));
        }


        private EventDto GetDtoFromEvent(Event _event)
        {
            return new EventDto(_event.Id.ToString(), _event.Title, _event.StartAt, _event.EndAt, _event.Description);
        }

        private Guid ParseStringId(string id)
        {
            if (!Guid.TryParse(id, out Guid result))
                throw new EventWrongParameterException("Строка Id не соответствует формату GUID");
            return result;
        }
    }
}

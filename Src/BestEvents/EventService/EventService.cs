
using BestEvents.Exceptions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BestEvents
{
    /// <summary>
    /// Сервис событий, реализующий интерфейс IEventService. 
    /// Использует репозиторий для получения данных и преобразования их 
    /// в Dto объекты для передачи в контроллеры и обратно
    /// </summary>
  
    public class EventService(IEventRepository repository, EventFilters filters, Pagination<Event> pagination) : IEventService
    {
        /// <summary>
        /// Создает новое событие, используя данные из Dto объекта и передает их в репозиторий для сохранения
        /// </summary>
        /// <param name="_event"></param>
        public EventInfoDto CreateEvent(CreateEventDto _event)
        {
            return GetDtoFromEvent(repository.AddEvent(Event.CreateNewEvent(_event.Title, _event.StartAt, _event.EndAt, _event.Description, _event.TotalSeats)));
        }

        /// <summary>
        /// Удаляет событие из репозитория по идентификатору 
        /// </summary>
        /// <param name="id"></param>
        public void DeleteEvent(string id)
        {
            if (!repository.RemoveEvent(ParseStringId(id)))
                throw new EventNotFoundException(string.Format(Messages_ru.EventNotDeleted, id));
        }

        /// <summary>
        /// Получает событие из репозитория по идентификатору, преобразует его в Dto объект и возвращает.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EventInfoDto GetEvent(string id)
        {
            var _event = repository.GetEvent(ParseStringId(id));
            if (_event == null)
                throw new EventNotFoundException(string.Format(Messages_ru.EventNotFound, id));
            return GetDtoFromEvent(_event);
        }

        /// <summary>
        /// Получает все события из репозитория, преобразует их в Dto объекты и возвращает в виде списка.
        /// </summary>
        /// <returns></returns>
        public PaginatedResultDto GetEvents(string? title, DateTime? from, DateTime? to, int page = 1, int size = 10)
        {
            if (from != null && to != null && from > to)
                throw new EventWrongParameterException(Messages_ru.EndAt_Less_StartAt);
            if (page <= 0)
                throw new EventWrongParameterException(string.Format(Messages_ru.WrongPageForPagination, page));
            if (size <= 0)
                throw new EventWrongParameterException(string.Format(Messages_ru.WrongSizeForPagination, size));

            IEnumerable<Event> events = repository.GetEvents();
            
            var filtredResult = filters.FilterEventsByTitle(events, title);
            filtredResult = filters.FilterEventsByDateFrom(filtredResult, from);
            filtredResult = filters.FilterEventsByDateTo(filtredResult, to);
            var result = pagination.GetResult(filtredResult, page, size);

            List<EventInfoDto> eventsDto = [];

            result.ResultsOnPage.ToList().ForEach(e => eventsDto.Add(GetDtoFromEvent(e)));

            return new PaginatedResultDto()
            {
                CurrentPage = result.CurrentPage,
                TotalResultsNumber = result.TotalResultsNumber,
                ResultsOnPage = eventsDto,
                ResultsNumberOnPage = result.ResultsNumberOnPage
            };
           
        }

        /// <summary>
        /// Перезаписывает событие в репозитории, используя данные из Dto объекта.
        /// </summary>
        /// <param name="eventDto"></param>
        /// /// <param name="id"></param>
        public void ReplaceEvent(string id, EventInfoDto eventDto)
        {
            if(id != eventDto.Id)
                throw new EventWrongParameterException(string.Format(Messages_ru.MismatchIdInReplaceRequest, id, eventDto.Id));
            if (eventDto.StartAt == null)
                throw new EventWrongParameterException(Messages_ru.No_StartAt);
            if (eventDto.EndAt == null)
                throw new EventWrongParameterException(Messages_ru.No_EndAt);
            var _event = Event.CreateEvent(ParseStringId(eventDto.Id), eventDto.Title, eventDto.StartAt.Value, eventDto.EndAt.Value, eventDto.Description, 
                eventDto.TotalSeats, eventDto.AvailableSeats);
            if(!repository.ReplaceEvent(_event))
                throw new EventNotFoundException(string.Format(Messages_ru.EventNotReplaced, _event.Id));
        }


        private EventInfoDto GetDtoFromEvent(Event _event)
        {
            return new EventInfoDto(_event.Id.ToString(), _event.Title, _event.StartAt, _event.EndAt, _event.Description, _event.TotalSeats, _event.AvailableSeats);
        }

        private Guid ParseStringId(string id)
        {
            if (!Guid.TryParse(id, out Guid result))
                throw new EventWrongParameterException(Messages_ru.Wrong_Id_Format);
            return result;
        }
    }
}

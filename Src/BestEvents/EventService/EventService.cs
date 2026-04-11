
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
        public EventInfo CreateEvent(CreateEvent _event)
        {
            return GetDtoFromEvent(repository.AddEvent(new Event(_event.Title, _event.StartAt, _event.EndAt, _event.Description)));
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
        public EventInfo GetEvent(string id)
        {
            return GetDtoFromEvent(repository.GetEvent(ParseStringId(id)));
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

            List<EventInfo> eventsDto = [];

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
        public void ReplaceEvent(string id, EventInfo eventDto)
        {
            if(id != eventDto.Id)
                throw new EventWrongParameterException(string.Format(Messages_ru.MismatchIdInReplaceRequest, id, eventDto.Id));
            if (eventDto.StartAt == null)
                throw new EventWrongParameterException(Messages_ru.No_StartAt);
            if (eventDto.EndAt == null)
                throw new EventWrongParameterException(Messages_ru.No_EndAt);
            var _event = new Event(ParseStringId(eventDto.Id), eventDto.Title, eventDto.StartAt.Value, eventDto.EndAt.Value, eventDto.Description);
            repository.ReplaceEvent(_event);
        }


        private EventInfo GetDtoFromEvent(Event _event)
        {
            return new EventInfo(_event.Id.ToString(), _event.Title, _event.StartAt, _event.EndAt, _event.Description);
        }

        private Guid ParseStringId(string id)
        {
            if (!Guid.TryParse(id, out Guid result))
                throw new EventWrongParameterException(Messages_ru.Wrong_Id_Format);
            return result;
        }
    }
}


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
        /// <inheritdoc/>
        public async Task<EventInfoDto> CreateEventAsync(CreateEventDto _event, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var newEvent = await repository.AddEventAsync(Event.CreateNewEvent(_event.Title, _event.StartAt, _event.EndAt, _event.Description, _event.TotalSeats));
            return GetDtoFromEvent(newEvent);
        }

        /// <inheritdoc/>
        public async Task DeleteEventAsync(string id, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            if (! await repository.RemoveEventAsync(ParseStringId(id)))
                throw new EventNotFoundException(string.Format(Messages_ru.EventNotDeleted, id));
        }

        /// <inheritdoc/>
        public async Task<EventInfoDto> GetEventAsync(string id, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var _event = await repository.GetEventAsync(ParseStringId(id));
            if (_event == null)
                throw new EventNotFoundException(string.Format(Messages_ru.EventNotFound, id));
            return GetDtoFromEvent(_event);
        }

        /// <inheritdoc/>
        public async Task<PaginatedResultDto> GetEventsAsync(string? title, DateTime? from, DateTime? to, int page = 1, int size = 10, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            if (from != null && to != null && from > to)
                throw new EventWrongParameterException(Messages_ru.EndAt_Less_StartAt);
            if (page <= 0)
                throw new EventWrongParameterException(string.Format(Messages_ru.WrongPageForPagination, page));
            if (size <= 0)
                throw new EventWrongParameterException(string.Format(Messages_ru.WrongSizeForPagination, size));

            IQueryable<Event> events = await repository.GetEventsAsync();
            
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

        /// <inheritdoc/>
        public async Task ReplaceEventAsync(string id, EventInfoDto eventDto, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            if (id != eventDto.Id)
                throw new EventWrongParameterException(string.Format(Messages_ru.MismatchIdInReplaceRequest, id, eventDto.Id));
            if (eventDto.StartAt == null)
                throw new EventWrongParameterException(Messages_ru.No_StartAt);
            if (eventDto.EndAt == null)
                throw new EventWrongParameterException(Messages_ru.No_EndAt);
            var _event = Event.CreateEvent(ParseStringId(eventDto.Id), eventDto.Title, eventDto.StartAt.Value, eventDto.EndAt.Value, eventDto.Description, 
                eventDto.TotalSeats, eventDto.AvailableSeats);
            if( !await repository.ReplaceEventAsync(_event))
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

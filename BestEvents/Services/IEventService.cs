namespace BestEvents
{
    /// <summary>
    /// Интерфейс сервиса работы с событиями
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Возвращает все события в виде списка EventDto
        /// </summary>
        /// <returns></returns>
        List<EventDto> GetEvents();

        /// <summary>
        /// Возвращает событие по его идентификатору в виде EventDto. Если событие с таким идентификатором не найдено, возвращает null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EventDto GetEvent(string id);

        /// <summary>
        /// Создает новое событие, используя данные из Dto объекта
        /// </summary>
        /// <param name="eventDto"></param>
        EventDto CreateEvent(EventDtoBase eventDto);

        /// <summary>
        /// Перезаписывает событие, используя данные из Dto объекта
        /// </summary>
        /// <param name="eventDto"></param>
        /// /// <param name="id"></param>
        void ReplaceEvent(string id, EventDto eventDto);

        /// <summary>
        /// Удаляет событие по его идентификатору
        /// </summary>
        /// <param name="id"></param>
        void DeleteEvent(string id);
    }
}

namespace BestEvents
{
    /// <summary>
    /// Интерфейс репозитория для хранения и управления событиями Event.
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Создает новое событие и сохраняет его в хранилище. Принимает название, дату начала, дату завершения и описание события в качестве параметров.
        /// </summary>
        /// <param name="title">Название события</param>
        /// <param name="startAt">Дата начала</param>
        /// <param name="endAt">Дата завершения</param>
        /// <param name="description">Опсание (необязательное поле)</param>
        Event AddEvent(string title, DateTime? startAt, DateTime? endAt, string? description);

        /// <summary>
        /// Удаление события из хранилища по его идентификатору.
        /// </summary>
        /// <param name="id"></param>
        void RemoveEvent(Guid id);

        /// <summary>
        /// Перезапиывает событие
        /// </summary>
        /// <param name="_event"></param>
        void ReplaceEvent(Event _event);

        /// <summary>
        /// Получает событие по его иденификатору. Если событие с таким идентификатором не найдено, возвращает null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Event GetEvent(Guid id);

        /// <summary>
        /// Получает все события из хранилища и возвращает их в виде списка.
        /// </summary>
        /// <returns></returns>
        PaginatedResult<Event> GetEvents(string? name, DateTime? from, DateTime? to, int page = 1, int size = 10);

        
    }
}

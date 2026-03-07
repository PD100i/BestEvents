using System.ComponentModel.DataAnnotations;

namespace BestEvents
{
    /// <summary>
    /// Dto класс, описывающий JSON структуру для http запросов на изменение и получение события
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <param name="title">Название события</param>
    /// <param name="startAt">Дата начала</param>
    /// <param name="endAt">Дата завершения</param>
    /// <param name="description">Описание (необязательный параметр)</param>
    public class EventDto(string id, string title, DateTime startAt, DateTime endAt, string? description) :
        EventDtoBase( title, startAt, endAt, description)    
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Необходимо задать параметр Id")]
        public string Id { get; set; } = id;

    }
}

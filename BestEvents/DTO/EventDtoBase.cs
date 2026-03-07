using System.ComponentModel.DataAnnotations;

namespace BestEvents
{
    /// <summary>
    /// Dto класс, описывающий JSON структуру для http запросов на создание события
    /// </summary>
    /// <param name="title">Название события</param>
    /// <param name="startAt">Дата начала</param>
    /// <param name="endAt">Дата завершения</param>
    /// <param name="description">Описание (необязательный параметр)</param>
    public class EventDtoBase(string title, DateTime? startAt, DateTime? endAt, string? description)
    {
        /// <summary>
        /// Название события
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Необходимо указать название события")]
        public string Title { get; set; } = title;

        /// <summary>
        /// Описание события
        /// </summary>
        public string? Description { get; set; } = description;

        /// <summary>
        /// Дата начала события
        /// </summary>
        [Required(ErrorMessage = "Не указана дата начала события")]
        [DataType(DataType.Date, ErrorMessage = "Неверный формат даты начала события")]
        public DateTime? StartAt { get; set; } = startAt;

        /// <summary>
        /// Дата завершения события
        /// </summary>
        [Required(ErrorMessage = "Не указана дата завершения мероприятия")]
        [DataType(DataType.Date, ErrorMessage = "Неверный формат даты завершения события")]
        public DateTime? EndAt { get; set; } = endAt;
    }
}

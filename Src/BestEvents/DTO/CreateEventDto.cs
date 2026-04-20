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
    /// <param name="totalSeats">Общее количество мест на событии</param>
    public class CreateEventDto(string title, DateTime? startAt, DateTime? endAt, string? description, int? totalSeats)
    {
        /// <summary>
        /// Название события
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "No_Title")]
        public string Title { get; set; } = title;

        /// <summary>
        /// Описание события
        /// </summary>
        public string? Description { get; set; } = description ?? "";

        /// <summary>
        /// Дата начала события
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "No_StartAt")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "WrongFormat_StartAt")]
        public DateTime? StartAt { get; set; } = startAt;

        /// <summary>
        /// Дата завершения события
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "No_EndAt")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "WrongFormat_EndAt")]
        public DateTime? EndAt { get; set; } = endAt;


        /// <summary>
        /// Общее количество мест на событии
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "No_TotalSeats")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "WrongEventTotalSeats")]
        public int? TotalSeats { get; } = totalSeats;

        
    }
}

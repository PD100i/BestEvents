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
    /// <param name="totalSeats">Общее количество мест на событии</param>
    /// <param name="availableSeats">Доступное количество мест на событии</param>
    public class EventInfo(string id, string title, DateTime? startAt, DateTime? endAt, string? description, int? totalSeats, int? availableSeats) 
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required( AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "No_Id")]
        public string Id { get; set; } = id;

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
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "TotalSeatsRangeOut")]
        public int? TotalSeats { get; } = totalSeats;


        /// <summary>
        /// Доступное количество мест
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "No_AvailableSeats")]
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "AvailableSeatsRangeOut")]
        public int? AvailableSeats { get; } = availableSeats;



    }
}

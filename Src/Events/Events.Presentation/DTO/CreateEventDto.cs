using System.ComponentModel.DataAnnotations;

namespace Events.Presentation
{
    /// <summary>
    /// Dto класс, описывающий JSON структуру для http запросов на создание события
    /// </summary>
    
    public class CreateEventDto()
    {
        /// <summary>
        /// Название события
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "No_Title")]
        public string Title { get; set; } = ""; 

        /// <summary>
        /// Описание события
        /// </summary>
        public string? Description { get; set; } = "";

        /// <summary>
        /// Дата начала события
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "No_StartAt")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "WrongFormat_StartAt")]
        public DateTime? StartAt { get; set; } 

        /// <summary>
        /// Дата завершения события
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "No_EndAt")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "WrongFormat_EndAt")]
        public DateTime? EndAt { get; set; } 


        /// <summary>
        /// Общее количество мест на событии
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "No_TotalSeats")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Messages_ru), ErrorMessageResourceName = "WrongEventTotalSeats")]
        public int? TotalSeats { get; set; } 
        
    }
}

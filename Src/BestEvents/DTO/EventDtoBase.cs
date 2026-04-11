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

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            EventDtoBase other = (EventDtoBase)obj;
            return Title == other.Title &&
                   Description == other.Description &&
                   StartAt == other.StartAt &&
                   EndAt == other.EndAt;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Description, StartAt, EndAt);

        }
    }
}

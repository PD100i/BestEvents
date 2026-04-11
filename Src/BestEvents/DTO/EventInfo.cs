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
    public class EventInfo(string id, string title, DateTime? startAt, DateTime? endAt, string? description) :
        CreateEvent(title, startAt, endAt, description)
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required(
            AllowEmptyStrings = false,
            ErrorMessageResourceType = typeof(Messages_ru),
            ErrorMessageResourceName = "No_Id")]
        public string Id { get; set; } = id;

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            EventInfo other = (EventInfo)obj;
            return Id == other.Id && base.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, base.GetHashCode());

        }

    }
}

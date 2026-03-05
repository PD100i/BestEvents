using System.ComponentModel.DataAnnotations;

namespace BestEvents
{
    public class Event
    {
        public Event(Guid id, string title, string Descriptor, DateTime startAt, DateTime endAt)
        {
            Id = id;
            Title = title;
            if (startAt > endAt)
                throw new ArgumentException("Завершение мероприятия не может быть позже его начала. Скорректируйте даты");
            StartAt = startAt;
            EndAt = endAt;
        }

        [Required(ErrorMessage = "Не задан Id")]
        public Guid Id {  get;  }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Необходимо указать название мероприятия")]
        public string Title {  get; set; }

        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Не указана дата начала мероприятия")]
        public DateTime StartAt { get; set; }

        [Required(ErrorMessage = "Не указана дата завершения мероприятия")]
        public DateTime EndAt { get; set; }

    }

    
}

using System.ComponentModel.DataAnnotations;

namespace BestEvents
{
    public record EventDto(string title, DateTime startAt, DateTime endAt, string description = "")
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Необходимо указать название мероприятия")]
        public string Title { get; set; } = title;

        [Required(AllowEmptyStrings = true, ErrorMessage = "Необходимо указать название мероприятия")]
        public string Description { get; set; } = description;

        [Required(ErrorMessage = "Не указана дата начала мероприятия")]
        public DateTime StartAt { get; set; } = startAt;

        [Required(ErrorMessage = "Не указана дата завершения мероприятия")]
        public DateTime EndAt { get; set; } = endAt;
    }
}

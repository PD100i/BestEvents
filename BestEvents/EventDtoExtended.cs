using System.ComponentModel.DataAnnotations;

namespace BestEvents
{
    public record EventDtoExtended(string id, string title, DateTime startAt, DateTime endAt, string description = "") :
        EventDto( title, startAt, endAt, description)    
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Не задан Id")]
        public string Id { get; set; } = id;

    }
}

using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.DTO
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public object Timestamp { get;  set; }
        public ApplicationUser User { get;  set; }
    }

}

using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.DTO
{
    public class NotificationDto
    {
        public ApplicationUser User { get; set; }
        public string Content { get; set; }
    }
}

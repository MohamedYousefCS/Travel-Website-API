using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.DTO
{
    public class GroupMessageDto
    {
        public string User { get; set; } // Assuming User is a string (e.g., username)

        public string Content { get; set; }

        public string GroupName { get; set; }
    }
}

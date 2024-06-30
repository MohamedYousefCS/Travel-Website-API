using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.DTO
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public object Timestamp { get;  set; }
      
        public string ProfilePictureUrl { get;  set; }
        public bool IsRead { get;  set; }
        public string Sender { get; set; }

        public string Receiver { get;  set; }
    }

}

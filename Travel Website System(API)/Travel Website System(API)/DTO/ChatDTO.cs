using System.ComponentModel.DataAnnotations.Schema;
using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.DTO
{
    public class ChatDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public bool IsDeleted { get; set; }



        public string? customerServiceId { get; set; }

        public string? clientId { get; set; }
    }
}

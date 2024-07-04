using System.ComponentModel.DataAnnotations;
using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.DTO
{

    public class ServiceProviderDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 20 characters")]
        public string Name { get; set; }

        [StringLength(20, ErrorMessage = "Description cannot exceed 20 characters")]
        public string Description { get; set; }

        [Url(ErrorMessage = "Logo must be a valid URL")]
        public string Logo { get; set; }

        public bool isDeleted { get; set; }

        public List<string> Services { get; set; }
    }
}

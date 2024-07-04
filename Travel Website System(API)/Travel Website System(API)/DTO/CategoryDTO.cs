using System.ComponentModel.DataAnnotations;
using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.DTO
{

    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 10 characters")]
        public string Name { get; set; }

        [StringLength(20, ErrorMessage = "Description cannot exceed 20 characters")]
        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public List<string> ServiceNames { get; set; }
    }
}

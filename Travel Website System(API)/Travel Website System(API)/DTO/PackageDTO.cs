using System.ComponentModel.DataAnnotations.Schema;
using Travel_Website_System_API.Models;
using System.ComponentModel.DataAnnotations;


namespace Travel_Website_System_API_.DTO
{
   

    public class PackageDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; } // For image upload

        public string ImageUrl { get; set; } // For returning the image URL

        [Range(0, int.MaxValue, ErrorMessage = "Quantity available must be a non-negative number")]
        public int? QuantityAvailable { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative number")]
        public decimal? Price { get; set; }
<<<<<<< HEAD
        public bool isDeleted { get; set; }    
=======

        public bool isDeleted { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid date format")]
>>>>>>> 4ac371bea6f60117308cc582fba0f211d50255b5
        public DateTime? startDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be a positive number")]
        public int? Duration { get; set; }

        //[Required(ErrorMessage = "Admin ID is required")]
        public string adminId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Booking time allowed must be a non-negative number")]
        public int? BookingTimeAllowed { get; set; }

        public List<string> ServiceNames { get; set; } = new List<string>(); 
    }
}

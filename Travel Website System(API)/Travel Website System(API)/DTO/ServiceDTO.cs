using Travel_Website_System_API.Models;
using ServiceProvider = Travel_Website_System_API.Models.ServiceProvider;
using System.ComponentModel.DataAnnotations;
using Travel_Website_System_API_.Models;


namespace Travel_Website_System_API_.DTO
{

    public class ServiceDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 30 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        //[Url(ErrorMessage = "Image must be a valid URL")]
        public string? Image { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity available must be a non-negative number")]
        public int? QuantityAvailable { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format")]
        public DateTime? StartDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative number")]
        public decimal? price { get; set; }

        public bool? isDeleted { get; set; }

        //[Required(ErrorMessage = "Category ID is required")]
        public int? categoryId { get; set; }

        //[Required(ErrorMessage = "Service Provider ID is required")]
        public int? serviceProviderId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Booking time allowed must be a non-negative number")]
        public int? BookingTimeAllowed { get; set; }//must <= start date
        public int? Duration { get; set; } = 1; // # nights = days-1,start+end
        public DateTime? EndDate { get; set; }//  choose



        // for flights
        public DateTime? GoingFlightTime { get; set; }// = start date of package (calculated) for view in front
        public DateTime? ComingFlightTime { get; set; }//= end date of pakcage (calculated)
        public string? GoingFlightSource { get; set; }
        public string? GoingFlightDestination { get; set; }
        public string? ComingBackFlightSource { get; set; }
        public string? ComingBackFlightDesination { get; set; }

        // Hotels

        public PackageLocationEnum? HoltelLocation { get; set; } // makkah , madinah

        public int? NumberOFAvailableRooms { get; set; }
        public int? NumberOFPersons { get; set; } = 2;// in each room 2 by default if increased add 1 room
                                                      // public decimal? TotalPrice { get; set; }

    }
}

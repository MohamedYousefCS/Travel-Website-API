using Travel_Website_System_API.Models;
using ServiceProvider = Travel_Website_System_API.Models.ServiceProvider;

namespace Travel_Website_System_API_.DTO
{
    public class ServiceDTO
    {

          public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int? QuantityAvailable { get; set; }
        public DateTime? StartDate { get; set; }
        public decimal? price { get; set; }
        public bool? isDeleted { get; set; }
     
        public virtual ICollection<BookingService> BookingServices { get; set; } = new HashSet<BookingService>();

        public virtual ICollection<LoveService> LoveServices { get; set; } = new HashSet<LoveService>();

     
        public virtual Category category { get; set; }


        public virtual ServiceProvider serviceProvider { get; set; }
              
        public virtual ICollection<Package> packages { get; set; } = new HashSet<Package>();
    


    }
}

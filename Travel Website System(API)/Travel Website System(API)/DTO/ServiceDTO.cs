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
          
        public  int? categoryId { get; set; }

        public  int? serviceProviderId { get; set; }
              
    


    }
}

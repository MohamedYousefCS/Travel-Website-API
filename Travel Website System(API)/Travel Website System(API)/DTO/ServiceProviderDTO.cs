using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.DTO
{
    public class ServiceProviderDTO
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public bool isDeleted { get; set; }
        public  List<string> Services { get; set; }

    }
}

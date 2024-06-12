using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Service> Services { get; set; }



    }
}

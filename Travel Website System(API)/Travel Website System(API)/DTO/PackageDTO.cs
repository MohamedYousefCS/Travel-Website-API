using System.ComponentModel.DataAnnotations.Schema;
using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.DTO
{
    public class PackageDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public int? QuantityAvailable { get; set; }
        public decimal? Price { get; set; }
        public bool isDeleted { get; set; }
        public DateTime? startDate { get; set; }
        public int? Duration { get; set; }
        public string? adminId { get; set; }

      

    }
}

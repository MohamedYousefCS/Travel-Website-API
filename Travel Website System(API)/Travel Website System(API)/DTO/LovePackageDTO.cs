using System.ComponentModel.DataAnnotations.Schema;
using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.DTO
{
    public class LovePackageDTO
    {
        public int Id { get; set; }
        public DateTime? date { get; set; }
        public bool IsDeleted { get; set; }

        public string clientId { get; set; }

        public int packageId { get; set; }

        public string PackageName { get; set; } 
        public string PackageDescription { get; set; } 
        public decimal? PackagePrice { get; set; }
    }
}

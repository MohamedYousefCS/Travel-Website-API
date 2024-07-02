using System.ComponentModel.DataAnnotations.Schema;
using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Models
{
    public class PackageService
    {
        public int PackageId { get; set; }
        public Package Package { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public DateTime AddedOn { get; set; }
    }
}

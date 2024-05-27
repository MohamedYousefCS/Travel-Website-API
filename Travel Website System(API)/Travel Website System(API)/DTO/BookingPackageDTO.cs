using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Travel_Website_System_API_.DTO
{
    public class BookingPackageDTO
    {
        [Key]
        public int BookingPackageId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }

        public int? quantity { get; set; }

        public int? clientId { get; set; }
        // i add it to  test relative data
        public string? clientName { get; set; }
        public decimal? Bookingprice { get; set; }

        public int? packageId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? allowingTime { get; set; }

    }
}


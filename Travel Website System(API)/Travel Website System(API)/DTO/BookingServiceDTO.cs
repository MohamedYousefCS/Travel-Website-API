﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Travel_Website_System_API_.DTO
{
    public class BookingServiceDTO
    {
        [Key]
        public int BookingServiceId { get; set; }
        public int? Quantity { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }
        public int? ServiceId { get; set; }
        public int? ClientId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? allowingTime { get; set; }
    }
}

﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Travel_Website_System_API_.DTO
{
    // in Dto i can have only properties which i need from model and i can add another properties which i need in uI
    public class BookingPackageDTO
    {
        [Key]
        public int? Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }

        public int? quantity { get; set; }

        public string? clientId { get; set; }
     
        public int? packageId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? allowingTime { get; set; }
        public Decimal  ?price { get; set; }
        public string ? PackageImage { get; set; }

        public string ? PackageName {  get; set; }


    }
}


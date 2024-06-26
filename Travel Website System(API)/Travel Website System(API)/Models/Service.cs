﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API_.Models;

namespace Travel_Website_System_API.Models
{

    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ?Description { get; set; }
        public string ?Image { get; set; }
        public int? QuantityAvailable { get; set; } = 4;
        public DateTime? StartDate { get; set; }
        public decimal? price { get; set; }
        public bool? isDeleted { get; set; }
     
        public virtual ICollection<BookingService> BookingServices { get; set; } = new HashSet<BookingService>();

        public virtual ICollection<LoveService> LoveServices { get; set; } = new HashSet<LoveService>();

        [ForeignKey("category")]
        public int? categoryId { get; set; }
        public virtual Category category { get; set; }

        [ForeignKey("serviceProvider")]
        public int? serviceProviderId { get; set; }
        public virtual ServiceProvider serviceProvider { get; set; }
              
       // public virtual ICollection<Package> Packages { get; set; } = new HashSet<Package>();

        public virtual ICollection<PackageService> PackageServices { get; set; } = new HashSet<PackageService>();

    }
}
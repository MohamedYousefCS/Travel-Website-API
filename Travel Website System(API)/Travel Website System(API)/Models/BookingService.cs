﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Travel_Website_System_API.Models;

[Table("BookingService")]
public partial class BookingService
{
    [Key]
    public int BookingServiceId { get; set; }

    public int? Quantity { get; set; }

    [Column(TypeName = "date")]
    public DateTime? Date { get; set; }

    public int? clientId { get; set; }

    public int? serviceId { get; set; }

    [Column(TypeName = "date")]
    public DateTime? allowingTime { get; set; }

    [InverseProperty("BookingService")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey("clientId")]
    [InverseProperty("BookingServices")]
    public virtual Client client { get; set; }

    [ForeignKey("serviceId")]
    [InverseProperty("BookingServices")]
    public virtual Service service { get; set; }
}
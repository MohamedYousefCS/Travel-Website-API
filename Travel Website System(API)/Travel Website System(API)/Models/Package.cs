﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Travel_Website_System_API.Models;

[Table("Package")]
public partial class Package
{
    [Key]
    public int packageId { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; }

    [Column(TypeName = "text")]
    [AllowNull] 
    public string ?Description { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    [AllowNull]
    public string ?Image { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TotalPrice { get; set; }

    public bool? isDeleted { get; set; }

    [Column(TypeName = "date")]
    public DateTime? startDate { get; set; }

    public int? Duration { get; set; }
    public int? BookingQuantity { get; set; } = 2;

    public int? adminId { get; set; }

    [InverseProperty("package")]
    public virtual ICollection<BookingPackage> BookingPackages { get; set; } = new List<BookingPackage>();

    [InverseProperty("package")]
    public virtual ICollection<LovePackage> LovePackages { get; set; } = new List<LovePackage>();

    [ForeignKey("adminId")]
    [InverseProperty("Packages")]
    public virtual Admin admin { get; set; }

    [ForeignKey("packageId")]
    [InverseProperty("packages")]
    public virtual ICollection<Service> services { get; set; } = new List<Service>();
}
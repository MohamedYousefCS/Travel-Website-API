﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Travel_Website_System_API.Models
{
   
    public class ApplicationUser : IdentityUser
    {
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Gender { get; set; }
        public string SSN { get; set; }
        public string Role { get; set; }
        public Admin? Admin { get; set; }
        public Client? client { get; set; }
        public bool IsDeleted { get; set; }

        public CustomerService? customerService { get; set; }


        public string Passport { get; set; }
        public string Location { get; set; }

        public override string UserName
        {
            get => Email;
            set => base.UserName = Email;
        }

    }
}


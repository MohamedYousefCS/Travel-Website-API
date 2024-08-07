﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Travel_Website_System_API.Models
{
    public class Message
    {
        [Key]

        public int Id { get; set; }


        public string status { get; set; }

        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
     
        public bool isDeleted { get; set; }

        //[ForeignKey("Chat")]
        public int? chatId { get; set; }
        public virtual Chat Chat { get; set; }

     

        public DateTime Timestamp { get;  set; }
        public string Content { get;  set; }
        public bool IsRead { get;  set; }
        public string ReceiverId { get; set; }
        //public ApplicationUser User { get; set; }
        public ApplicationUser Receiver { get;  set; }
        public string GroupName { get; internal set; }
    }
}
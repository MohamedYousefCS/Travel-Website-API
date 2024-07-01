using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace Travel_Website_System_API.Models
{

    public class ClientConnection
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ConnectionId { get; set; }
        public bool IsConnected { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}

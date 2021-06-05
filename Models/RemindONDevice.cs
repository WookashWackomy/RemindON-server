using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace RemindONServer.Models
{
    public class RemindONDevice
    {
        [Key]
        public string SerialNumber { get; set; }
        public string UserId { get; set; }

        [Display(Name = "Last seen")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastSeen { get; set; }
        public string Password { get; set; }
    }
}

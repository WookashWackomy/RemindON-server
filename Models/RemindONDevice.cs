using System;
using System.ComponentModel.DataAnnotations;

namespace RemindONServer.Models
{
    public class RemindONDevice
    {
        public int ID { get; set; }
        public string SerialNumber { get; set; }
        public string UserId { get; set; }

        [Display(Name = "Last seen")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastSeen { get; set; }
    }
}

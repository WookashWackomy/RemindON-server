using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Models
{
    public class RemindONDeviceViewModel
    {
        public string SerialNumber { get; set; }
        public string UserId { get; set; }
        public DateTime LastSeen { get; set; }
        public string Description { get; set; }
    }
}

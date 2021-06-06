using RemindONServer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Models
{
    public class CheckViewModel
    {
        public int ID { get; set; }
        public int PrescriptionID { get; set; }
        public bool Flag { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}

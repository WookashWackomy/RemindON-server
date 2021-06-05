using RemindONServer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Models
{
    public class Check
    {
        public int ID { get; set; }
        public int PrescriptionID { get; set; }
        public Flag Flag { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}

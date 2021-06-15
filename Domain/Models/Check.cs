using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Models
{
    public class Check
    {
        public int ID { get; set; }
        public int PrescriptionID { get; set; }
        public bool Flag { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}

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
        public int Flag { get; set; }
        public string TimeStamp { get; set; }
    }
}

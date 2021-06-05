using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Models
{
    public class PrescriptionViewModel
    {
        public string text1 { get; set; }
        public string text2 { get; set; }
        public List<DayOfWeek> WeekDays { get; set; }
        public List<DateTime> DayTimes { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Models
{
    public class PrescriptionViewModel
    {
        public string text1 { get; set; }
        public string text2 { get; set; }
        public List<DayOfWeek> WeekDays { get; set; }
        public IEnumerable<string> DayTimes { get; set; }
        public int ID { get; internal set; }
    }
}

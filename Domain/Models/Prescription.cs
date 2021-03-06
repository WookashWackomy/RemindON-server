using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RemindONServer.Domain.Models
{
    public class Prescription
    {
        public int ID { get; set; }
        public string DeviceSerialNumber { get; set; }
        public string text1 { get; set; }
        public string text2 { get; set; }
        public List<DayOfWeek> WeekDays { get; set; }
        public List<TimeSpan> DayTimes { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Models
{
    public class EmbeddedTimeStampHelper
    {
        public static DateTime Epoch { get; } = new(2020, 1, 1);
        public static string GetFormattedDate(DateTime dateTime) => $"{dateTime:yyyy-MM-dd HH:mm:ss} {(int)dateTime.DayOfWeek}";
        public static string GetFormattedDateReduced(DateTime dateTime) => $"{dateTime.Subtract(Epoch).Days},{(int)dateTime.DayOfWeek},{(int)dateTime.TimeOfDay.TotalSeconds}";
    }
}

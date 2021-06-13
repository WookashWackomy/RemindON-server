using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Models
{
    public class EmbeddedPrescriptionViewModel : PrescriptionViewModel
    {
        public override string ToString()
        {
            return $"{ID},{text1},{text2},[{string.Join(',',WeekDays.Select(wd => (int)wd))}],[{string.Join(',',DayTimes)}]";
        }
    }
}

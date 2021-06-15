using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Models.ModelMappers
{
    public class PrescriptionModelMapper
    {
        public static PrescriptionViewModel Map(Prescription prescription)
        {
            return new PrescriptionViewModel
            {
                text1 = prescription.text1,
                text2 = prescription.text2,
                WeekDays = prescription.WeekDays,
                DayTimes = prescription.DayTimes.Select(prescriptionDayTime => prescriptionDayTime.ToString()),
                ID = prescription.ID
            };
        }

        public static Prescription MapFromViewModel(PrescriptionViewModel prescriptionViewModel)
        {
            return new Prescription
            {
                ID = prescriptionViewModel.ID,
                text1 = prescriptionViewModel.text1,
                text2 = prescriptionViewModel.text2,
                WeekDays = prescriptionViewModel.WeekDays,
                DayTimes = prescriptionViewModel.DayTimes.Select(dt => TimeSpan.Parse(dt)).ToList()
            };
        }

        public static EmbeddedPrescriptionViewModel MapEmbedded(Prescription prescription)
        {
            return new EmbeddedPrescriptionViewModel
            {
                text1 = prescription.text1,
                text2 = prescription.text2,
                WeekDays = prescription.WeekDays,
                DayTimes = prescription.DayTimes.Select(prescriptionDayTime => prescriptionDayTime.ToString()),
                ID = prescription.ID
            };
        }

        public static Prescription MapFromEmeddedVievModel(EmbeddedPrescriptionViewModel embeddedPrescriptionViewModel)
        {
            return new Prescription
            {
                ID = embeddedPrescriptionViewModel.ID,
                text1 = embeddedPrescriptionViewModel.text1,
                text2 = embeddedPrescriptionViewModel.text2,
                WeekDays = embeddedPrescriptionViewModel.WeekDays,
                DayTimes = embeddedPrescriptionViewModel.DayTimes.Select(dt => TimeSpan.Parse(dt)).ToList()
            };
        }
    }
}

using RemindONServer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Services.Communication
{
    public class SavePrescriptionResponse : BaseResponse
    {
        public Prescription Prescription { get; private set; }

        private SavePrescriptionResponse(bool success, string message, Prescription prescription) : base(success, message)
        {
            Prescription = prescription;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="prescription">Saved prescription.</param>
        /// <returns>Response.</returns>
        public SavePrescriptionResponse(Prescription prescription) : this(true, string.Empty, prescription)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public SavePrescriptionResponse(string message) : this(false, message, null)
        { }
    }
}

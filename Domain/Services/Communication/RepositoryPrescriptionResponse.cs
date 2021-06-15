using RemindONServer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Services.Communication
{
    public class RepositoryPrescriptionResponse : BaseResponse
    {
        public Prescription Prescription { get; private set; }

        private RepositoryPrescriptionResponse(RepositoryResponse repositoryResponse, string message, Prescription prescription) : base(repositoryResponse, message)
        {
            Prescription = prescription;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="prescription">Saved prescription.</param>
        /// <returns>Response.</returns>
        public RepositoryPrescriptionResponse(Prescription prescription) : this(RepositoryResponse.Success, string.Empty, prescription)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RepositoryPrescriptionResponse(RepositoryResponse repositoryResponse, string message) : this(repositoryResponse, message, null)
        { }
    }
}

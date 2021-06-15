using RemindONServer.Domain.Models;
using RemindONServer.Domain.Services.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Services
{
    public interface IPrescriptionsService
    {
        Task<IEnumerable<Prescription>> ListAsync();
        Task<SavePrescriptionResponse> SaveAsync(Prescription prescription);
    }
}

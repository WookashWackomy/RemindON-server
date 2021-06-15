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
        Task<RepositoryPrescriptionResponse> SaveAsync(Prescription prescription);
        Task<RepositoryPrescriptionResponse> UpdateAsync(int id, Prescription category);
        Task<RepositoryPrescriptionResponse> DeleteAsync(int id);
    }
}

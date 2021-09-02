using RemindONServer.Domain.Models;
using RemindONServer.Domain.Services.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Services
{
    public interface IPrescriptionsService
    {
        Task<RepositoryResponse<IEnumerable<Prescription>>> ListAsync();
        Task<RepositoryResponse<IEnumerable<Prescription>>> ListByDeviceSerialNumberAsync(string serialNumber);
        Task<RepositoryResponse<IEnumerable<Prescription>>> ListByDeviceSerialNumberAndDayOfWeekAsync(string serialNumber, DayOfWeek dayOfWeek);
        Task<RepositoryResponse<Prescription>> SaveAsync(Prescription prescription);
        Task<RepositoryResponse<Prescription>> UpdateAsync(int id, Prescription category);
        Task<RepositoryResponse<Prescription>> DeleteAsync(int id); 
        Task<RepositoryResponse<Prescription>> GetByIdAsync(int id);
    }
}

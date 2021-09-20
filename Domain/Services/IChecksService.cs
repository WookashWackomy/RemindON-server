using RemindONServer.Domain.Models;
using RemindONServer.Domain.Services.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Services
{
    public interface IChecksService
    {
        Task<RepositoryResponse<IEnumerable<Check>>> ListAsync();
        Task<RepositoryResponse<IEnumerable<Check>>> ListByDeviceSerialNumberAsync(string serialNumber);
        Task<RepositoryResponse<IEnumerable<Check>>> ListByDeviceSerialNumberAndDayOfWeekAsync(string serialNumber, DayOfWeek dayOfWeek);
        Task<RepositoryResponse<Check>> SaveAsync(Check check);
        Task<RepositoryResponse<Check>> UpdateAsync(int id, Check check);
        Task<RepositoryResponse<Check>> DeleteAsync(int id);
        Task<RepositoryResponse<Check>> GetByIdAsync(int id);
    }
}

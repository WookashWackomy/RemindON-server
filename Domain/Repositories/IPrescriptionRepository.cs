using RemindONServer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Repositories
{
    public interface IPrescriptionRepository
    {
        Task<IEnumerable<Prescription>> ListAsync(); 
        Task AddAsync(Prescription prescription);
        Task<Prescription> FindByIdAsync(int id);
        void Update(Prescription prescription);
        void Remove(Prescription prescription);
    }
}

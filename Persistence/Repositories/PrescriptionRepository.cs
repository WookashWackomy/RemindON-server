using Microsoft.EntityFrameworkCore;
using RemindONServer.Domain.Models;
using RemindONServer.Domain.Persistence.Contexts;
using RemindONServer.Domain.Repositories;
using RemindONServer.Persistence;
using RemindONServer.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Persistence.Repositories
{
    public class PrescriptionRepository : BaseRepository, IPrescriptionRepository
    {
        public PrescriptionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Prescription>> ListAsync()
        {
            return await _context.Prescriptions
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task AddAsync(Prescription prescription)
        {
            await _context.Prescriptions.AddAsync(prescription);
        }
        public async Task<Prescription> FindByIdAsync(int id)
        {
            return await _context.Prescriptions.FindAsync(id);
        }

        public void Update(Prescription prescription)
        {
            _context.Prescriptions.Update(prescription);
        }

        public void Remove(Prescription prescription)
        {
            _context.Prescriptions.Remove(prescription);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using RemindONServer.Domain.Models;
using RemindONServer.Domain.Persistence.Contexts;
using RemindONServer.Domain.Repositories;
using RemindONServer.Domain.Services.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Services
{
    //TODO exception logger
    public class PrescriptionService : IPrescriptionsService
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<RepositoryResponse<IEnumerable<Prescription>>> ListAsync()
        {
            return new RepositoryResponse<IEnumerable<Prescription>>(await _context.Prescriptions.ToListAsync());
        }
        public async Task<RepositoryResponse<IEnumerable<Prescription>>> ListByDeviceSerialNumberAsync(string serialNumber)
        {
            return new RepositoryResponse<IEnumerable<Prescription>>(await _context.Prescriptions.Where(p => p.DeviceSerialNumber == serialNumber).ToListAsync());
        }
        public async Task<RepositoryResponse<IEnumerable<Prescription>>> ListByDeviceSerialNumberAndDayOfWeekAsync(string serialNumber, DayOfWeek dayOfWeek)
        {
            return new RepositoryResponse<IEnumerable<Prescription>>(await _context.Prescriptions.Where(p => p.DeviceSerialNumber == serialNumber && p.WeekDays.Contains(dayOfWeek)).ToListAsync());
        }

        public async Task<RepositoryResponse<Prescription>> GetByIdAsync(int id)
        {
            try
            {
                var prescription = await _context.Prescriptions.FirstAsync(p => p.ID == id);
                return new RepositoryResponse<Prescription>(prescription);
            } catch (InvalidOperationException _)
            {
                return new RepositoryResponse<Prescription>(RepositoryResponseType.NotFound, "Prescription not found.");
            }
        }

        public async Task<RepositoryResponse<Prescription>> SaveAsync(Prescription prescription)
        {
            try
            {
                await _context.Prescriptions.AddAsync(prescription);
                await _context.SaveChangesAsync();
                return new RepositoryResponse<Prescription>(prescription);
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Prescription>(RepositoryResponseType.Error, $"An error occurred when saving the prescription: {ex.Message}");
            }
        }

        public async Task<RepositoryResponse<Prescription>> UpdateAsync(int id, Prescription prescription)
        {
            var existingPrescription = (await GetByIdAsync(id)).Resource;

            try
            {
                existingPrescription.text1 = prescription.text1;
            existingPrescription.text2 = prescription.text2;
            existingPrescription.WeekDays = prescription.WeekDays;
            existingPrescription.DayTimes = prescription.DayTimes;
                await _context.SaveChangesAsync();
                return new RepositoryResponse<Prescription>(existingPrescription);
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Prescription>(RepositoryResponseType.Error, $"An error occurred when updating the category: {ex.Message}");
            }
        }
        public async Task<RepositoryResponse<Prescription>> DeleteAsync(int id)
        {
            var existingPrescription = (await GetByIdAsync(id)).Resource;
            try
            {
                _context.Prescriptions.Remove(existingPrescription);
                await _context.SaveChangesAsync();

                return new RepositoryResponse<Prescription>(existingPrescription);
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Prescription>(RepositoryResponseType.Error, $"An error occurred when deleting the category: {ex.Message}");
            }
        }
    }
}

using RemindONServer.Domain.Models;
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
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PrescriptionService(IPrescriptionRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _prescriptionRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<Prescription>> ListAsync()
        {
            return await _prescriptionRepository.ListAsync();
        }

        public async Task<RepositoryPrescriptionResponse> SaveAsync(Prescription prescription)
        {
            try
            {
                await _prescriptionRepository.AddAsync(prescription);
                await _unitOfWork.CompleteAsync();

                return new RepositoryPrescriptionResponse(prescription);
            }
            catch (Exception ex)
            {
                return new RepositoryPrescriptionResponse(RepositoryResponse.Error,$"An error occurred when saving the prescription: {ex.Message}");
            }
        }

        public async Task<RepositoryPrescriptionResponse> UpdateAsync(int id, Prescription prescription)
        {
            var existingPrescription = await _prescriptionRepository.FindByIdAsync(id);

            if (existingPrescription == null)
                return new RepositoryPrescriptionResponse(RepositoryResponse.NotFound,"Category not found.");

            existingPrescription.text1 = prescription.text1;
            existingPrescription.text2 = prescription.text2;
            existingPrescription.WeekDays = prescription.WeekDays;
            existingPrescription.DayTimes = prescription.DayTimes;

            try
            {
                await _unitOfWork.CompleteAsync();

                return new RepositoryPrescriptionResponse(existingPrescription);
            }
            catch (Exception ex)
            {
                return new RepositoryPrescriptionResponse(RepositoryResponse.Error, $"An error occurred when updating the category: {ex.Message}");
            }
        }
        public async Task<RepositoryPrescriptionResponse> DeleteAsync(int id)
        {
            var existingCategory = await _prescriptionRepository.FindByIdAsync(id);

            if (existingCategory == null)
                return new RepositoryPrescriptionResponse(RepositoryResponse.NotFound, "Category not found.");

            try
            {
                _prescriptionRepository.Remove(existingCategory);
                await _unitOfWork.CompleteAsync();

                return new RepositoryPrescriptionResponse(existingCategory);
            }
            catch (Exception ex)
            {
                return new RepositoryPrescriptionResponse(RepositoryResponse.Error, $"An error occurred when deleting the category: {ex.Message}");
            }
        }
    }
}

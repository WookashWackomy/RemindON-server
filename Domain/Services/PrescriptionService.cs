using RemindONServer.Domain.Models;
using RemindONServer.Domain.Repositories;
using RemindONServer.Domain.Services.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Services
{
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

        public async Task<SavePrescriptionResponse> SaveAsync(Prescription prescription)
        {
            try
            {
                await _prescriptionRepository.AddAsync(prescription);
                await _unitOfWork.CompleteAsync();

                return new SavePrescriptionResponse(prescription);
            }
            catch (Exception ex)
            {
                //TODO logger
                return new SavePrescriptionResponse($"An error occurred when saving the prescription: {ex.Message}");
            }
        }
    }
}

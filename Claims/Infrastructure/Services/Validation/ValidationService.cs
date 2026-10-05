using Claims.Infrastructure.Repository.Insurance;
using Claims.Domain.Models.Insurance;

namespace Claims.Infrastructure.Services.Validation
{
    /// <summary>
    /// Validation service for checking if a claim or cover can be correctly added.
    /// </summary>
    public class ValidationService : IValidationService
    {
        private readonly IInsuranceRepository insuranceRepository;
        private const int MaxDamageCost = 100000;
        private const int MaxCoverDurationDays = 365;

        public ValidationService(IInsuranceRepository insuranceRepository) 
        {
            this.insuranceRepository = insuranceRepository;
        }

        public async Task<bool> ValidateClaim(Claim claim)
        {
            if (claim.DamageCost > MaxDamageCost)
            {
                return false;
            }

            Cover? cover = await insuranceRepository.GetCoverByIdAsync(claim.CoverId ?? 0);
            if (cover is not null)
            {
                if (claim.Created < cover.StartDate || claim.Created > cover.EndDate)
                {
                    return false;
                }

            }

            return true;
        }

        public bool ValidateCover(Cover cover)
        {
            if (cover.StartDate < DateTime.Now)
            {
                return false;
            }

            if ((cover.EndDate - cover.StartDate).TotalDays > MaxCoverDurationDays)
            {
                return false;
            }

            return true;
        }
    }
}

using Claims.Domain.Exceptions;
using Claims.Domain.Models.Insurance;
using Claims.Infrastructure.Repository.Insurance;

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

        public async Task ValidateClaim(Claim claim)
        {
            if (claim.DamageCost > MaxDamageCost)
            {
                throw new BadValidationException($"Damage cost exceeds the maximum - {MaxDamageCost}", StatusCodes.Status400BadRequest);
            }

            Cover? cover = await insuranceRepository.GetCoverByIdAsync(claim.CoverId ?? 0);
            if (cover is not null)
            {
                if (claim.Created < cover.StartDate || claim.Created > cover.EndDate)
                {
                    throw new BadValidationException($"Claim date is outside the cover period - {cover.StartDate} to {cover.EndDate}", StatusCodes.Status400BadRequest);
                }

            }
        }

        public void ValidateCover(Cover cover)
        {
            if (cover.StartDate < DateTime.Now)
            {
                throw new BadValidationException($"Cover start date is in the past - {cover.StartDate}", StatusCodes.Status400BadRequest);
            }

            if ((cover.EndDate - cover.StartDate).TotalDays > MaxCoverDurationDays)
            {
                throw new BadValidationException($"Cover duration exceeds the maximum - {MaxCoverDurationDays} days", StatusCodes.Status400BadRequest);
            }
        }
    }
}

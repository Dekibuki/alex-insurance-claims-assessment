using Claims.Domain.Enums;
using Claims.Domain.Models.Audit;
using Claims.Infrastructure.Repository.Audit;

namespace Claims.Infrastructure.Services.Audit
{
    /// <summary>
    /// Service for managing auditing of claims and covers.
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository auditRepository;

        public AuditService(IAuditRepository auditRepository)
        {
            this.auditRepository = auditRepository;
        }

        public async Task AuditClaim(int id, string httpRequestType)
        {
            ClaimAudit claimAudit = new ClaimAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                ClaimId = id
            };

            await auditRepository.AddAuditClaimAsync(claimAudit);
        }
        
        public async Task AuditCover(int id, string httpRequestType)
        {
            CoverAudit coverAudit = new CoverAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                CoverId = id
            };

            await auditRepository.AddAuditCoverAsync(coverAudit);
        }

        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            decimal multiplier = 1.3m;

            decimal yachtMultiplier = 1.1m;
            decimal passengerShipMultiplier = 1.2m;
            decimal tankerMultiplier = 1.5m;

            int startingPremium = 1250;

            if (coverType == CoverType.Yacht)
            {
                multiplier = yachtMultiplier;
            }

            if (coverType == CoverType.PassengerShip)
            {
                multiplier = passengerShipMultiplier;
            }

            if (coverType == CoverType.Tanker)
            {
                multiplier = tankerMultiplier;
            }

            decimal premiumPerDay = startingPremium * multiplier;
            double insuranceLength = (endDate - startDate).TotalDays;
            decimal totalPremium = CalculatePremiumFromInsuranceLength(insuranceLength, premiumPerDay, coverType);

            return totalPremium;
        }

        private decimal CalculatePremiumFromInsuranceLength(double insuranceLength, decimal premiumPerDay, CoverType coverType)
        {
            decimal totalPremium = 0m;
            for (int i = 0; i < insuranceLength; i++)
            {
                if (i < 30)
                    totalPremium += premiumPerDay;
                else if (i > 30 &&i < 180 && coverType == CoverType.Yacht)
                    totalPremium += premiumPerDay - premiumPerDay * 0.05m;
                else if (i > 30 && i < 180)
                    totalPremium += premiumPerDay - premiumPerDay * 0.02m;
                else if (i > 180 && i < 365 && coverType != CoverType.Yacht)
                    totalPremium += premiumPerDay - premiumPerDay * 0.03m;
                else if (i > 180 && i < 365)
                    totalPremium += premiumPerDay - premiumPerDay * 0.01m;
            }
            return totalPremium;
        }
    }
}

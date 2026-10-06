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

        private const decimal DefaultMultiplier = 1.3m;
        private const decimal YachtMultiplier = 1.1m;
        private const decimal PassengerShipMultiplier = 1.2m;
        private const decimal TankerMultiplier = 1.5m;

        private const int StartingPremium = 1250;

        private const int FirstPeriodDays = 30;
        private const int SecondPeriodDays = 180;
        private const int ThirdPeriodDays = 365;


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

        /// <summary>
        /// Computes the premium for a given cover type and insurance period.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="coverType"></param>
        /// <returns></returns>
        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            decimal multiplier = DefaultMultiplier;

            if (coverType == CoverType.Yacht)
            {
                multiplier = YachtMultiplier;
            }

            if (coverType == CoverType.PassengerShip)
            {
                multiplier = PassengerShipMultiplier;
            }

            if (coverType == CoverType.Tanker)
            {
                multiplier = TankerMultiplier;
            }

            decimal premiumPerDay = StartingPremium * multiplier;
            double insuranceLength = (endDate - startDate).TotalDays;
            decimal totalPremium = CalculatePremiumFromInsuranceLength(insuranceLength, premiumPerDay, coverType);

            return totalPremium;
        }

        private decimal CalculatePremiumFromInsuranceLength(double insuranceLength, decimal premiumPerDay, CoverType coverType)
        {
            decimal totalPremium = 0m;
            for (int i = 0; i < insuranceLength; i++)
            {
                if (i < FirstPeriodDays)
                    totalPremium += premiumPerDay;
                else if (i > FirstPeriodDays && i < SecondPeriodDays && coverType == CoverType.Yacht)
                    totalPremium += premiumPerDay - premiumPerDay * 0.05m;
                else if (i > FirstPeriodDays && i < SecondPeriodDays)
                    totalPremium += premiumPerDay - premiumPerDay * 0.02m;
                else if (i > SecondPeriodDays && i < ThirdPeriodDays && coverType != CoverType.Yacht)
                    totalPremium += premiumPerDay - premiumPerDay * 0.03m;
                else if (i > SecondPeriodDays && i < ThirdPeriodDays)
                    totalPremium += premiumPerDay - premiumPerDay * 0.01m;
            }
            return totalPremium;
        }
    }
}

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
    }
}

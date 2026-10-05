using Claims.Domain.Models.Audit;

namespace Claims.Infrastructure.Repository.Audit
{
    /// <summary>
    /// Repository interface for managing audits in the database.
    /// Currently provides only create operations for audits.
    /// </summary>
    public interface IAuditRepository
    {
        public Task AddAuditCoverAsync(CoverAudit item);

        public Task AddAuditClaimAsync(ClaimAudit item);
    }
}

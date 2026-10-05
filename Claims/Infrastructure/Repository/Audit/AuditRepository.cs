using Claims.Domain.Models.Audit;
using Claims.Infrastructure.DataContext;

namespace Claims.Infrastructure.Repository.Audit
{
    /// <summary>
    /// Repository class for managing audits in the database.
    /// Currently provides only create operations for audits.
    /// </summary>
    public class AuditRepository : IAuditRepository
    {
        private readonly MainContext _auditContext;

        public AuditRepository(MainContext auditContext)
        {
            _auditContext = auditContext;
        }

        public Task AddAuditCoverAsync(CoverAudit item)
        {
            _auditContext.CoverAudits.Add(item);
            return _auditContext.SaveChangesAsync();
        }

        public Task AddAuditClaimAsync(ClaimAudit item)
        {
            _auditContext.ClaimAudits.Add(item);
            return _auditContext.SaveChangesAsync();
        }
    }
}

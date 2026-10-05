using Claims.Domain.Enums;

namespace Claims.Infrastructure.Services.Audit
{
    /// <summary>
    /// Interface for managing auditing of claims and covers.
    /// </summary>
    public interface IAuditService
    {
        public Task AuditClaim(string id, string httpRequestType);

        public Task AuditCover(string id, string httpRequestType);

        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
    }
}

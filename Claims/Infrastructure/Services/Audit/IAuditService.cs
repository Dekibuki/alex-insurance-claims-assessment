using Claims.Domain.Enums;

namespace Claims.Infrastructure.Services.Audit
{
    /// <summary>
    /// Interface for managing auditing of claims and covers.
    /// </summary>
    public interface IAuditService
    {
        public Task AuditClaim(int id, string httpRequestType);

        public Task AuditCover(int id, string httpRequestType);
    }
}

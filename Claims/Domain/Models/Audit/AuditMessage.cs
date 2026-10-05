using Claims.Domain.Enums;

namespace Claims.Domain.Models.Audit
{
    public class AuditMessage
    {
        public AuditType Type { get; set; }
        public int EntityId { get; set; }
        public string HttpRequestType { get; set; } = string.Empty;
        public DateTime Created { get; set; }
    }
}

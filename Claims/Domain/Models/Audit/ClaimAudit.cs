namespace Claims.Domain.Models.Audit
{
    public class ClaimAudit
    {
        public int Id { get; set; }

        public int? ClaimId { get; set; }

        public DateTime Created { get; set; }

        public string? HttpRequestType { get; set; }
    }
}

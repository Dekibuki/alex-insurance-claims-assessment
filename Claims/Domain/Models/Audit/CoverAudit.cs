namespace Claims.Domain.Models.Audit
{
    public class CoverAudit
    {
        public int Id { get; set; }

        public int? CoverId { get; set; }

        public DateTime Created { get; set; }

        public string? HttpRequestType { get; set; }
    }
}

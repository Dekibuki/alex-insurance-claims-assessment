using Claims.Domain.Enums;

namespace Claims.Domain.Models.Insurance
{
    public class ComputePremiumDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CoverType Type { get; set; }
    }
}

using MongoDB.Bson.Serialization.Attributes;
using Claims.Domain.Enums;

namespace Claims.Domain.Models.Insurance
{
    public class Claim
    {
        public int Id { get; set; }

        public int? CoverId { get; set; }

        public DateTime Created { get; set; }

        public string Name { get; set; } = string.Empty;

        public ClaimType Type { get; set; }

        public decimal DamageCost { get; set; }
    }
}

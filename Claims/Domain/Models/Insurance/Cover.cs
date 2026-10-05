using MongoDB.Bson.Serialization.Attributes;
using Claims.Domain.Enums;

namespace Claims.Domain.Models.Insurance
{
    public class Cover
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public CoverType Type { get; set; }

        public decimal Premium { get; set; }
    }
}

using MongoDB.Bson.Serialization.Attributes;
using Claims.Domain.Enums;

namespace Claims.Domain.Models.Insurance;

public class Cover
{
    [BsonId]
    public string Id { get; set; }

    [BsonElement("startDate")]
    [BsonDateTimeOptions(DateOnly = true)]
    public DateTime StartDate { get; set; }

    [BsonElement("endDate")]
    [BsonDateTimeOptions(DateOnly = true)]
    public DateTime EndDate { get; set; }

    [BsonElement("claimType")]
    public CoverType Type { get; set; }

    [BsonElement("premium")]
    public decimal Premium { get; set; }
}

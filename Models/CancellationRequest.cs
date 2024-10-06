using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD_Web_Service_API.Models
{
    public class CancellationRequest
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("customer_email"), BsonRepresentation(BsonType.String)]
        public string Customer_Email { get; set; }
        [BsonElement("customer_id"), BsonRepresentation(BsonType.String)]
        public string Customer_Id { get; set; }
        [BsonElement("odrder_id"), BsonRepresentation(BsonType.String)]
        public string Order_Id { get; set; }
        [BsonElement("body"), BsonRepresentation(BsonType.String)]
        public string Body { get; set; }
        [BsonElement("status"), BsonRepresentation(BsonType.String)]
        public string? Status { get; set; }
        [BsonElement("created_at"), BsonRepresentation(BsonType.DateTime)]
        public DateTime Created_At { get; set; }
    }
}

using EAD_Web_Service_API.Data;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD_Web_Service_API.Models
{
    public class Notification
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("receiver_id"), BsonRepresentation(BsonType.String)]
        public string Receiver_Id { get; set; }
        [BsonElement("subject"), BsonRepresentation(BsonType.String)]
        public string Subject { get; set; }
        [BsonElement("body"), BsonRepresentation(BsonType.String)]
        public string Body { get; set; }
        [BsonElement("viewed"), BsonRepresentation(BsonType.Int32)]
        public bool Viewed { get; set; }
    }
}

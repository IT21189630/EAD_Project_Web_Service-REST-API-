// ---------------------------------------------------------------------------
// File: Notification.cs
// Author: IT21189630
// Date Created: 2024-10-01
// Description: This class contains all the properties related to a notification.
// Version: 1.0.0
// ---------------------------------------------------------------------------
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
        [BsonElement("viewed"), BsonRepresentation(BsonType.Boolean)]
        public bool Viewed { get; set; }
        [BsonElement("created_at"), BsonRepresentation(BsonType.DateTime)]
        public DateTime Created_At { get; set; }
    }
}

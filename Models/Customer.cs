using EAD_Web_Service_API.Data;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD_Web_Service_API.Models
{
    public class Customer
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("username"), BsonRepresentation(BsonType.String)]
        public string Username { get; set; }
        [BsonElement("email"), BsonRepresentation(BsonType.String)]
        public string Email { get; set; }
        [BsonElement("password"), BsonRepresentation(BsonType.String)]
        public string Password { get; set; }
        [BsonElement("address"), BsonRepresentation(BsonType.String)]
        public string Address { get; set; }
        [BsonElement("profile_picture"), BsonRepresentation(BsonType.String)]
        public string? Profile_Picture { get; set; }
        [BsonElement("acitvation_status"), BsonRepresentation(BsonType.Boolean)]
        public bool Activation_Status { get; set; }
    }
}

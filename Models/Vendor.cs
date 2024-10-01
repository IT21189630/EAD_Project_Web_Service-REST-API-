using EAD_Web_Service_API.Data;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD_Web_Service_API.Models
{
    public class Vendor : IUser
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("username"), BsonRepresentation(BsonType.String)]
        public string Username { get; set; }
        [BsonElement("nic"), BsonRepresentation(BsonType.String)]
        public string NIC { get; set; }
        [BsonElement("email"), BsonRepresentation(BsonType.String)]
        public string Email { get; set; }
        [BsonElement("password"), BsonRepresentation(BsonType.String)]
        public string? Password { get; set; }
        [BsonElement("role"), BsonRepresentation(BsonType.Int32)]
        public UserRoles Role { get; set; }
        [BsonElement("profile_picture"), BsonRepresentation(BsonType.String)]
        public string? Profile_Picture { get; set; }
        [BsonElement("first_login"), BsonRepresentation(BsonType.Boolean)]
        public bool First_Login { get; set; }
        [BsonElement("status"), BsonRepresentation(BsonType.Boolean)]
        public bool Status { get; set; }
    }
}

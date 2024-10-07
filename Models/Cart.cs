using EAD_Web_Service_API.Data;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD_Web_Service_API.Models
{
    public class Cart
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("product_name"), BsonRepresentation(BsonType.String)]
        public string Product_Name { get; set; }

        [BsonElement("price"), BsonRepresentation(BsonType.Double)]
        public double Price { get; set; }

        [BsonElement("number_of_items"), BsonRepresentation(BsonType.Int32)]
        public int Number_Of_Items { get; set; }

        [BsonElement("image"), BsonRepresentation(BsonType.String)]
        public string? Image { get; set; }

    }
}

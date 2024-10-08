// ---------------------------------------------------------------------------
// File: Product.cs
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
    public class Product
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("product_name"), BsonRepresentation(BsonType.String)]
        public string Product_Name { get; set; }
        [BsonElement("product_category"), BsonRepresentation(BsonType.String)]
        public string Product_Category { get; set; }
        [BsonElement("vendor_name"), BsonRepresentation(BsonType.String)]
        public string Vendor_Name { get; set; }
        [BsonElement("vendor_id"), BsonRepresentation(BsonType.String)]
        public string Vendor_Id { get; set; }
        [BsonElement("image"), BsonRepresentation(BsonType.String)]
        public string? Image { get; set; }
        [BsonElement("description"), BsonRepresentation(BsonType.String)]
        public string Description { get; set; }
        [BsonElement("price"), BsonRepresentation(BsonType.Double)]
        public double Price { get; set; }
        [BsonElement("stock"), BsonRepresentation(BsonType.Int32)]
        public int Stock { get; set; }
        [BsonElement("visibility"), BsonRepresentation(BsonType.Boolean)]
        public bool Visibility { get; set; }
    }
}

// ---------------------------------------------------------------------------
// File: Cart.cs
// Author: IT21211164
// Date Created: 2024-09-30
// Description: This file is a class. It contains the properties related to a cartItem.
// Version: 1.0.0
// ---------------------------------------------------------------------------

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

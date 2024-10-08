// ---------------------------------------------------------------------------
// File: Category.cs
// Author: IT21211164
// Date Created: 2024-09-30
// Description: This file is a class. It contains the properties related to a category.
// Version: 1.0.0
// ---------------------------------------------------------------------------

using EAD_Web_Service_API.Data;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD_Web_Service_API.Models
{
    public class Category
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("category_name"), BsonRepresentation(BsonType.String)]
        public string Category_Name { get; set; }

        [BsonElement("title"), BsonRepresentation(BsonType.String)]
        public string Title { get; set; }

        [BsonElement("image"), BsonRepresentation(BsonType.String)]
        public string? Image { get; set; }
    }
}

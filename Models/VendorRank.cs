// ---------------------------------------------------------------------------
// File: Vendor.cs
// Author: IT21189630
// Date Created: 2024-09-29
// Description: This class contains all the properties related to a vendor.
// Version: 1.0.0
// ---------------------------------------------------------------------------
using EAD_Web_Service_API.Data;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace EAD_Web_Service_API.Models
{
    public class VendorRank
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("vendor_id"), BsonRepresentation(BsonType.String)]
        public string Vendor_Id { get; set; }

        [BsonElement("vendor_name"), BsonRepresentation(BsonType.String)]
        public string Vendor_Name { get; set; }

        [BsonElement("average_rating"), BsonRepresentation(BsonType.Double)]
        public double Average_Rating { get; set; }

        [BsonElement("total_ratings"), BsonRepresentation(BsonType.Int32)]
        public int? Total_Ratings { get; set; }

        [BsonElement("reviews")]
        public List<Review> Reviews { get; set; }
    }
    public class Review
    {
        [BsonElement("user_id"), BsonRepresentation(BsonType.ObjectId)]
        public string User_Id { get; set; }

        [BsonElement("rating"), BsonRepresentation(BsonType.Int32)]
        public int Rating { get; set; }
    }
}


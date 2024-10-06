using EAD_Web_Service_API.Data;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD_Web_Service_API.Models
{
    public class Order
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("customer_name"), BsonRepresentation(BsonType.String)]
        public string Customer_Name { get; set; }
        [BsonElement("customer_id"), BsonRepresentation(BsonType.String)]
        public string Customer_Id { get; set; }
        [BsonElement("product_id"), BsonRepresentation(BsonType.String)]
        public string Product_Id { get; set; }
        [BsonElement("quantity"), BsonRepresentation(BsonType.Int32)]
        public int Quantity { get; set; }   
        [BsonElement("vendor_id"), BsonRepresentation(BsonType.String)]
        public string Vendor_Id { get; set; }
        [BsonElement("status"), BsonRepresentation(BsonType.String)]
        public string? Status { get; set; }
        [BsonElement("total"), BsonRepresentation(BsonType.Double)]
        public double Total { get; set; }
        [BsonElement("created_at"), BsonRepresentation(BsonType.DateTime)]
        public DateTime Created_At { get; set; }
    }
}

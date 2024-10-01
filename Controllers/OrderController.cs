using EAD_Web_Service_API.Data;
using EAD_Web_Service_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EAD_Web_Service_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMongoCollection<Order> _orders;
        public OrderController(MongoDBService mongoDBService) {
            _orders = mongoDBService.database.GetCollection<Order>("orders");
        }

        //get orders by vendor id
        [HttpGet("vendor/{id}")]
        public async Task<ActionResult<List<Order>>> GetOrdersByVendorId(string id)
        {
            var filter = Builders<Order>.Filter.Eq(order => order.Vendor_Id, id);

            var orders = await _orders.Find(filter).ToListAsync();

            if (orders.Count > 0)
            {
                return Ok(orders);
            }
            return NotFound("No orders yet.");
        }
    }
}

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
        private readonly IMongoCollection<Notification> _notifications;
        private readonly IMongoCollection<Product> _products;

        public OrderController(MongoDBService mongoDBService) {
            _orders = mongoDBService.database.GetCollection<Order>("orders");
            _notifications = mongoDBService.database.GetCollection<Notification>("notifications");
            _products = mongoDBService.database.GetCollection<Product>("products");
        }

        //get all orders
        [HttpGet]
        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _orders.Find(FilterDefinition<Order>.Empty).ToListAsync();
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

        //send notification(deliver the order)
        private async Task SendDeliverNotification(string receiverId, string productId)
        {
            var filter = Builders<Product>.Filter.Eq(prd => prd.Id, productId);
            var product = await _products.Find(filter).FirstOrDefaultAsync();

            var notification = new Notification
            {
                Receiver_Id = receiverId,
                Subject = "Order Delivered!",
                Body = $"This message is to notify you that product {product.Product_Name} has been delivered to you!",
                Viewed = false,
                Created_At = DateTime.UtcNow,
            };

            await _notifications.InsertOneAsync(notification);
        }

        //create new order
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            order.Id = null;
            order.Created_At = DateTime.UtcNow;
            order.Status = "processing";
            await _orders.InsertOneAsync(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(string id)
        {
            var filter = Builders<Order>.Filter.Eq(order => order.Id, id);
            var order = await _orders.Find(filter).FirstOrDefaultAsync();

            if (order != null)
            {
                return Ok(order);
            }

            return NotFound();
        }

        //cancel order
        [HttpPut("cancel_order/{id}")]
        public async Task<ActionResult> CancelOrder(string id)
        {
            var filter = Builders<Order>.Filter.Eq(order => order.Id, id);
            var order = await _orders.Find(filter).FirstOrDefaultAsync();

            if (order != null)
            {
                if(order.Status == "processing")
                {
                    order.Status = "cancelled";
                    await _orders.ReplaceOneAsync(filter, order);
                    return Ok();
                }
                else
                {
                    return BadRequest("Target order is already dispatched or delivered");
                }
            }

            return BadRequest("Order state can not be changed!");
        }

        //dispatch order
        [HttpPut("dispatch_order/{id}")]
        public async Task<ActionResult> DispatchOrder(string id)
        {
            var filter = Builders<Order>.Filter.Eq(order => order.Id, id);
            var order = await _orders.Find(filter).FirstOrDefaultAsync();

            if (order != null)
            {
                if (order.Status == "processing")
                {
                    order.Status = "dispatched";
                    await _orders.ReplaceOneAsync(filter, order);
                    return Ok();
                }
                else
                {
                    return BadRequest("Target order is not in processing state");
                }
            }

            return BadRequest("Order state can not be changed!");
        }

        //deliver order
        [HttpPut("deliver_order/{id}")]
        public async Task<ActionResult> DeliverOrder(string id)
        {
            var filter = Builders<Order>.Filter.Eq(order => order.Id, id);
            var order = await _orders.Find(filter).FirstOrDefaultAsync();

            if (order != null)
            {
                 if(order.Status == "dispatched")
                {
                    order.Status = "delivered";
                    await _orders.ReplaceOneAsync(filter, order);
                    await SendDeliverNotification(order.Customer_Id, order.Product_Id);
                    return Ok();
                }
                else
                {
                    return BadRequest("Target order is not in dispatched state.");
                }
            }

            return BadRequest("Order state can not be changed!");
        }
    }
}

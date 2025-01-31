﻿// ---------------------------------------------------------------------------
// File: OrderController.cs
// Author: IT21189630, IT21189494, IT21211164
// Date Created: 2024-10-01
// Description: This file contains the logic for handling order management 
//              operations such as retrieving, adding, state updating an order.
// Version: 1.0.0
// ---------------------------------------------------------------------------
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

        //get all orders (by IT21189494)
        [HttpGet]
        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _orders.Find(FilterDefinition<Order>.Empty).ToListAsync();
        }

        //get orders by vendor id (by IT21189494)
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

        //send notification(deliver the order) - by IT21189630
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


        //send notification(cancelling the order) - for customer (by IT21189630)
        private async Task SendCancelOrderNotificationForCustomer(string receiverId, string productId, string note)
        {
            var filter = Builders<Product>.Filter.Eq(prd => prd.Id, productId);
            var product = await _products.Find(filter).FirstOrDefaultAsync();

            var notification = new Notification
            {
                Receiver_Id = receiverId,
                Subject = "Order Cancelled!",
                Body = $"This message is to notify you that product {product.Product_Name} order has been cancelled! We informed the relevant vendor as the reason for cancellation is {note}.",
                Viewed = false,
                Created_At = DateTime.UtcNow,
            };

            await _notifications.InsertOneAsync(notification);
        }

        //send notification(cancelling the order) - for vendor (by IT21189630)
        private async Task SendCancelOrderNotificationForVendor(string receiverId, string orderId, string note)
        {
            var notification = new Notification
            {
                Receiver_Id = receiverId,
                Subject = "Order Cancelled!",
                Body = $"This message is to notify you that order {orderId} order has been cancelled! reason for cancellation is {note}. sorry for the inconvenience.",
                Viewed = false,
                Created_At = DateTime.UtcNow,
            };

            await _notifications.InsertOneAsync(notification);
        }

        //create new order (by IT21211164)
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            order.Id = null;
            order.Created_At = DateTime.UtcNow;
            order.Status = "processing";
            await _orders.InsertOneAsync(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        //get an order using object id (by IT21211164)
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

        //cancel order (by IT21189630)
        [HttpPut("cancel_order/{id}")]
        public async Task<ActionResult> CancelOrder(string id, [FromBody] string note)
        {
            var filter = Builders<Order>.Filter.Eq(order => order.Id, id);
            var order = await _orders.Find(filter).FirstOrDefaultAsync();

            if (order != null)
            {
                if(order.Status == "processing")
                {
                    order.Status = "cancelled";
                    await _orders.ReplaceOneAsync(filter, order);
                    await SendCancelOrderNotificationForCustomer(order.Customer_Id, order.Product_Id, note);
                    await SendCancelOrderNotificationForVendor(order.Vendor_Id, order.Id, note);
                    return Ok();
                }
                else
                {
                    return BadRequest("Target order is already dispatched or delivered");
                }
            }
            return BadRequest("Order state can not be changed!");
        }

        //dispatch order (by IT21189630)
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

        //deliver order (by IT21189630)
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

// ---------------------------------------------------------------------------
// File: NotificationController.cs
// Author: IT21189630
// Date Created: 2024-10-01
// Description: This file contains the logic for dispatch notifications, retrieve
//              notifications using receiver id and object id.
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
    public class NotificationController : ControllerBase
    {
        private readonly IMongoCollection<Notification> _notifications;
        public NotificationController(MongoDBService mongoDBService) {
            _notifications = mongoDBService.database.GetCollection<Notification>("notifications");
        }


        //create new notification
        [HttpPost]
        public async Task<ActionResult> SendNotification(Notification notification)
        {
            await _notifications.InsertOneAsync(notification);
            return CreatedAtAction(nameof(GetNotificationById), new { id = notification.Id }, notification);
        }

        //get notification by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotificationById(string id)
        {
            var filter = Builders<Notification>.Filter.Eq(msg => msg.Id, id);
            var notification = await _notifications.Find(filter).FirstOrDefaultAsync();

            if (notification != null)
            {
                return Ok(notification);
            }

            return NotFound();
        }

        //get all notifications belong to a user
        [HttpGet("user/{id}")]
        public async Task<ActionResult<List<Notification>>> GetNotificationsByReceiver(string id)
        {
            var filter = Builders<Notification>.Filter.Eq(msg => msg.Receiver_Id, id);
            var notifications = await _notifications.Find(filter).ToListAsync();

            return Ok(notifications.Count > 0 ? notifications : new List<Notification>());
        }
    }
}

// ---------------------------------------------------------------------------
// File: CustomerController.cs
// Author: IT21189630, IT21189494, IT21211164
// Date Created: 2024-10-06
// Description: This file contains the logic for handling customer management 
//              operations such as retrieving, adding, updating, and deleting customers.
//              also contains the logic for account activation status management and notification dispatch
// Version: 1.0.0
// ---------------------------------------------------------------------------
using Amazon.Runtime.Internal;
using EAD_Web_Service_API.Data;
using EAD_Web_Service_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EAD_Web_Service_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMongoCollection<Customer> _customers;
        private readonly IMongoCollection<Notification> _notifications;

        public CustomerController(MongoDBService mongoDBService) 
        {
            _customers = mongoDBService.database.GetCollection<Customer>("customers");
            _notifications = mongoDBService.database.GetCollection<Notification>("notifications");
        }

        //get all customers (by IT21189494)
        [HttpGet]
        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            return await _customers.Find(FilterDefinition<Customer>.Empty).ToListAsync();
        }

        //create a new customer (by IT21189494)
        [HttpPost]
        public async Task<ActionResult> CreateCustomer(Customer customer)
        {
            var customerFilter = Builders<Customer>.Filter.Eq(cust => cust.Email, customer.Email);
            var duplicateCustomer = await _customers.Find(customerFilter).FirstOrDefaultAsync();

            if (duplicateCustomer != null)
            {
                return BadRequest("Provided email is already registered to customer!");
            }

            customer.Id = null;
            customer.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(customer.Password, 10);
            customer.Profile_Picture = "sample_pic";
            customer.Activation_Status = true;
            await _customers.InsertOneAsync(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
        }

        //get customer details by object id (by IT21211164)
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(string id)
        {
            var filter = Builders<Customer>.Filter.Eq(customer => customer.Id, id);
            var customer = await _customers.Find(filter).FirstOrDefaultAsync();

            if (customer != null)
            {
                return Ok(customer);
            }

            return NotFound();
        }

        //handle the notification mechansim when customer account reactivated (by IT21189630)
        private async Task SendNotification(string receiverId)
        {
            var notification = new Notification
            {
                Receiver_Id = receiverId,
                Subject = "Your Account Activation Status Changed!",
                Body = "CSR has changed the activation status of your account account!",
                Viewed = false,
                Created_At = DateTime.UtcNow,
            };

            await _notifications.InsertOneAsync(notification);
        }

        // deactivate customer account (by IT21189630)
        [HttpPut("deactivate/{id}")]
        public async Task<ActionResult> DeactivateAccount(string id)
        {
            var filter = Builders<Customer>.Filter.Eq(customer => customer.Id, id);
            var customer = await _customers.Find(filter).FirstOrDefaultAsync();

            if (customer != null)
            {
                customer.Activation_Status = false;
                await _customers.ReplaceOneAsync(filter, customer);
                return Ok();
            }

            return BadRequest("Customer activation status can not be updated!");
        }

        // activate or deactivate customer account (by IT21189630)
        [HttpPut("activate/{id}")]
        public async Task<ActionResult> ActivateAccount(string id)
        {
            var filter = Builders<Customer>.Filter.Eq(customer => customer.Id, id);
            var customer = await _customers.Find(filter).FirstOrDefaultAsync();

            if (customer != null)
            {
                customer.Activation_Status = !customer.Activation_Status;
                await _customers.ReplaceOneAsync(filter, customer);

                await SendNotification(customer.Id);

                return Ok();
            }

            return BadRequest("Customer activation status can not be updated!");
        }

        // get all customer accounts which are in deactivated state (by IT21189630)
        [HttpGet("deactivated")]
        public async Task<ActionResult<List<Customer>>> GetDeactivatedAccounts()
        {
            var filter = Builders<Customer>.Filter.Eq(customer => customer.Activation_Status, false);
            var deactivatedCustomers = await _customers.Find(filter).ToListAsync();

            if (deactivatedCustomers.Count > 0)
            {
                return Ok(deactivatedCustomers);
            }

            return Ok(new List<Customer>());
        }


        [HttpPost("login")]
        public async Task<ActionResult<Customer>> Login(LoginRequest loginRequest)
        {
            var user = Builders<Customer>.Filter.Eq(customer => customer.Email, loginRequest.Email);
            var customer = await _customers.Find(user).FirstOrDefaultAsync();

            if (customer == null)
            {
                return Unauthorized("Invalid username or password");
            }
            else if (BCrypt.Net.BCrypt.EnhancedVerify(loginRequest.Password, customer.Password))
            {
                return Ok(customer);
            }
            else
            {
                return BadRequest("Invalid username or password");
            }
        }
    }
}

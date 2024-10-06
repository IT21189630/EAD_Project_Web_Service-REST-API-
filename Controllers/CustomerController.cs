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

        [HttpGet]
        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            return await _customers.Find(FilterDefinition<Customer>.Empty).ToListAsync();
        }

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

        private async Task SendNotification(string receiverId)
        {
            var notification = new Notification
            {
                Receiver_Id = receiverId,
                Subject = "Your Account Activation Status Changed!",
                Body = "CSR has reactivated ypur account! Now you can use your account.",
                Viewed = false,
                Created_At = DateTime.UtcNow,
            };

            await _notifications.InsertOneAsync(notification);
        }

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
    }
}

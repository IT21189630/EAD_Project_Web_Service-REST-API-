using BCrypt.Net;
using EAD_Web_Service_API.Data;
using EAD_Web_Service_API.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace EAD_Web_Service_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAuthController : ControllerBase
    {
        private readonly IMongoCollection<Customer> _customers;

        public CustomerAuthController(MongoDBService mongoDBService)
        {
            _customers = mongoDBService.database.GetCollection<Customer>("customers");
        }

        // Login function for customers
        [HttpPost]
        public async Task<ActionResult<CustomerLoginResponse>> CustomerLogin(UserLoginRequest request)
        {
            // Check if the user exists in the customers collection
            var customerFilter = Builders<Customer>.Filter.Eq(customer => customer.Email, request.email);
            var customer = await _customers.Find(customerFilter).FirstOrDefaultAsync();

            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            // Verify the password
            if (!BCrypt.Net.BCrypt.EnhancedVerify(request.password, customer.Password))
            {
                return BadRequest("Email and password do not match");
            }

            // Check activation status
            if (!customer.Activation_Status)
            {
                return BadRequest("Your account is not activated yet.");
            }

            // Return the necessary fields for the customer login
            return new CustomerLoginResponse
            {
                Id = customer.Id,
                Username = customer.Username,
                Email = customer.Email,
                Address = customer.Address,
                Profile_Picture = customer.Profile_Picture,
                Activation_Status = customer.Activation_Status
            };
        }
    }
}

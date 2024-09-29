using BCrypt.Net;
using EAD_Web_Service_API.Data;
using EAD_Web_Service_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EAD_Web_Service_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private readonly IMongoCollection<Admin> _admins;

        public AdminAuthController(MongoDBService mongoDBService)
        {
            _admins = mongoDBService.database.GetCollection<Admin>("admins");
        }

        [HttpPost]
        public async Task<ActionResult<AdminLoginRespone>> Login(string email, string password)
        {
            var filter = Builders<Admin>.Filter.Eq(admin => admin.Email, email);          
            var admin = await _admins.Find(filter).FirstOrDefaultAsync();
            if (admin == null) {
                return NotFound($"Admin with email address {email} not found!");
            }

            if(BCrypt.Net.BCrypt.EnhancedVerify(password, admin.Password))
            {
                return new AdminLoginRespone
                {
                    Username = admin.Username,
                    Email = admin.Email,
                    Profile_Picture = admin.Profile_Picture,
                    Role = admin.Role
                };
            }
            else
            {
                return BadRequest($"Admin email and password mismatch");
            }
        }
    }
}

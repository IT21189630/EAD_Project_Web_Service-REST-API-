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
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<Admin> _admins;
        private readonly IMongoCollection<Vendor> _vendors;
        private readonly IMongoCollection<CustomerSalesRep> _csrs;

        public AuthController(MongoDBService mongoDBService)
        {
            _admins = mongoDBService.database.GetCollection<Admin>("admins");
            _vendors = mongoDBService.database.GetCollection<Vendor>("vendors");
            _csrs = mongoDBService.database.GetCollection<CustomerSalesRep>("csrs");
        }

        [HttpPost]
        public async Task<ActionResult<UserLoginRespone>> Login(UserLoginRequest request)
        {

            // check if requested user is an admin
            var adminFilter = Builders<Admin>.Filter.Eq(admin => admin.Email, request.email);
            IUser user = await _admins.Find(adminFilter).FirstOrDefaultAsync();

            // if user is not an admin, check in vendors collection
            if (user == null) { 
                var vendorFilter = Builders<Vendor>.Filter.Eq(vendor => vendor.Email, request.email);
                user = await _vendors.Find(vendorFilter).FirstOrDefaultAsync();
            }

            // if user is not an vendor, check in csr collection
            if (user == null)
            {
                var csrFilter = Builders<CustomerSalesRep>.Filter.Eq(csr => csr.Email, request.email);
                user = await _csrs.Find(csrFilter).FirstOrDefaultAsync();
            }

            // in case user email is not registered in any collection
            if (user == null) { 
                return NotFound($"user not found");
            }

            // verify the password
            if (BCrypt.Net.BCrypt.EnhancedVerify(request.password, user.Password))
            {
                return new UserLoginRespone
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Profile_Picture = user.Profile_Picture,
                    Role = user.Role
                }; 
            }
            else
            {
                return BadRequest($"user email and password mismatch");
            }
        }
    }
}

using EAD_Web_Service_API.Data;
using EAD_Web_Service_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Runtime.ConstrainedExecution;

namespace EAD_Web_Service_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IMongoCollection<Vendor> _vendors;

        public VendorController(MongoDBService mongoDBService) {
            _vendors = mongoDBService.database.GetCollection<Vendor>("vendors");
        }

        [HttpPost]
        public async Task<ActionResult> createVendor(Vendor vendor)
        {
            vendor.Id = null;
            vendor.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(vendor.NIC, 10);
            vendor.Role = UserRoles.VENDOR;
            vendor.Profile_Picture = UserProfiles.Profiles["Vendor"];
            vendor.First_Login = true;
            vendor.Status = true;
            await _vendors.InsertOneAsync(vendor);
            return CreatedAtAction(nameof(getVendorById), new { id = vendor.Id }, vendor);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> getVendorById(string id)
        {
            var filter = Builders<Vendor>.Filter.Eq(vendor => vendor.Id, id);
            var vendor = await _vendors.Find(filter).FirstOrDefaultAsync();

            if (vendor == null) { 
                return NotFound();
            }

            return Ok(vendor);
        }
    }
}

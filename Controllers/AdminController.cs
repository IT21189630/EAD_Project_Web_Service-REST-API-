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
    public class AdminController : ControllerBase
    {
        private readonly IMongoCollection<Admin> _admins;

        public AdminController(MongoDBService mongoDBService)
        {
            _admins = mongoDBService.database.GetCollection<Admin>("admins");
        }

        [HttpGet]
        public async Task<IEnumerable<Admin>> GetAdmins()
        {
            return await _admins.Find(FilterDefinition<Admin>.Empty).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdminById(string id)
        {
            var filter = Builders<Admin>.Filter.Eq(admin => admin.Id, id);
            var admin = _admins.Find(filter).FirstOrDefault();

            if (admin != null)
            {
                return Ok(admin);
            }

            return NotFound();
        }


        [HttpPost]
        public async Task<ActionResult> CreateAdmin(Admin admin)
        {
            admin.Id = null;
            admin.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(admin.Password, 10);
            admin.Role = UserRoles.ADMIN;
            admin.Profile_Picture = UserProfiles.Profiles["Admin"];
            await _admins.InsertOneAsync(admin);
            return CreatedAtAction(nameof(GetAdminById), new {id = admin.Id}, admin);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAdmin(Admin admin, string id)
        {
            var filter = Builders<Admin>.Filter.Eq(adm => adm.Id, id);
            await _admins.ReplaceOneAsync(filter, admin);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAdmin(string id)
        {
            var filter = Builders<Admin>.Filter.Eq(adm => adm.Id, id);
            await _admins.DeleteOneAsync(filter);
            return Ok();
        }
    }
}

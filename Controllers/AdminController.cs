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
        private readonly IMongoCollection<CustomerSalesRep> _csrs;
        private readonly IMongoCollection<Vendor> _vendors;

        public AdminController(MongoDBService mongoDBService)
        {
            _admins = mongoDBService.database.GetCollection<Admin>("admins");
            _csrs = mongoDBService.database.GetCollection<CustomerSalesRep>("csrs");
            _vendors = mongoDBService.database.GetCollection<Vendor>("vendors");
        }

        // get all administrators
        [HttpGet]
        public async Task<IEnumerable<Admin>> GetAdmins()
        {
            return await _admins.Find(FilterDefinition<Admin>.Empty).ToListAsync();
        }

        // get administrator by object id
        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdminById(string id)
        {
            var filter = Builders<Admin>.Filter.Eq(admin => admin.Id, id);
            var admin = await _admins.Find(filter).FirstOrDefaultAsync();

            if (admin != null)
            {
                return Ok(admin);
            }

            return NotFound();
        }

        // create new administrator account
        [HttpPost]
        public async Task<ActionResult> CreateAdmin(Admin admin)
        {
            // check if email is in admin collection
            var adminFilter = Builders<Admin>.Filter.Eq(adm => adm.Email, admin.Email);
            var duplicateAdmin = await _admins.Find(adminFilter).FirstOrDefaultAsync();

            if (duplicateAdmin != null) { 
                return BadRequest("Provided email is belong to an admin!");
            }

            // check if email is in vendor collection
            var vendorFilter = Builders<Vendor>.Filter.Eq(ven => ven.Email, admin.Email);
            var duplicateVendor = await _vendors.Find(vendorFilter).FirstOrDefaultAsync();

            if (duplicateVendor != null)
            {
                return BadRequest("Provided email is belong to an vendor!");
            }

            // check if email is in csr collection
            var SalesRepFilter = Builders<CustomerSalesRep>.Filter.Eq(csr => csr.Email, admin.Email);
            var duplicateCSR = await _csrs.Find(SalesRepFilter).FirstOrDefaultAsync();

            if (duplicateVendor != null)
            {
                return BadRequest("Provided email is belong to an csr!");
            }

            admin.Id = null;
            admin.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(admin.Password, 10);
            admin.Role = UserRoles.ADMIN;
            admin.Profile_Picture = UserProfiles.Profiles["Admin"];
            await _admins.InsertOneAsync(admin);
            return CreatedAtAction(nameof(GetAdminById), new {id = admin.Id}, admin);
        }

        // update an admin using the object id
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAdmin(Admin admin, string id)
        {
            var filter = Builders<Admin>.Filter.Eq(adm => adm.Id, id);
            await _admins.ReplaceOneAsync(filter, admin);
            return Ok();
        }

        // delete an admin using the object id
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAdmin(string id)
        {
            var filter = Builders<Admin>.Filter.Eq(adm => adm.Id, id);
            await _admins.DeleteOneAsync(filter);
            return Ok();
        }
    }
}

using EAD_Web_Service_API.Data;
using EAD_Web_Service_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EAD_Web_Service_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CSRController : ControllerBase
    {
        private readonly IMongoCollection<CustomerSalesRep> _csrs;
        public CSRController(MongoDBService mongoDBService) {
            _csrs = mongoDBService.database.GetCollection<CustomerSalesRep>("csrs");
        }

        [HttpPost]
        public async Task<ActionResult> createCustomerSalesRep(CustomerSalesRep csr)
        {
            csr.Id = null;
            csr.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(csr.NIC, 10);
            csr.Role = UserRoles.CSR;
            csr.Profile_Picture = UserProfiles.Profiles["CSR"];
            csr.First_Login = true;
            csr.Status = true;
            await _csrs.InsertOneAsync(csr);
            return CreatedAtAction(nameof(getSalesRepById), new { id = csr.Id }, csr);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerSalesRep>> getSalesRepById(string id)
        {
            var filter = Builders<CustomerSalesRep>.Filter.Eq(csr => csr.Id, id);
            var sales_rep = await _csrs.Find(filter).FirstOrDefaultAsync();

            if (sales_rep != null)
            {
                return Ok(sales_rep);
            }

            return NotFound();
        }
    }
}

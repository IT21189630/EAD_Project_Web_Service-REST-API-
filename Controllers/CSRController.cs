// ---------------------------------------------------------------------------
// File: CSRController.cs
// Author: IT21189630
// Date Created: 2024-09-29
// Description: This file contains the logic for handling customer sales representatives (CSR) management 
//              operations such as retrieving, adding, updating, and deleting CSR.
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
    public class CSRController : ControllerBase
    {
        private readonly IMongoCollection<CustomerSalesRep> _csrs;
        private readonly IMongoCollection<Admin> _admins;
        private readonly IMongoCollection<Vendor> _vendors;

        public CSRController(MongoDBService mongoDBService) {
            _csrs = mongoDBService.database.GetCollection<CustomerSalesRep>("csrs");
            _vendors = mongoDBService.database.GetCollection<Vendor>("vendors");
            _admins = mongoDBService.database.GetCollection<Admin>("admins");
        }

        //get all CSR accounts
        [HttpGet]
        public async Task<List<CustomerSalesRep>> getVendors()
        {
            return await _csrs.Find(FilterDefinition<CustomerSalesRep>.Empty).ToListAsync();
        }

        //create new customer CSR account
        [HttpPost]
        public async Task<ActionResult> createCustomerSalesRep(CustomerSalesRep csr)
        {

            // check if email is in admin collection
            var adminFilter = Builders<Admin>.Filter.Eq(adm => adm.Email, csr.Email);
            var duplicateAdmin = await _admins.Find(adminFilter).FirstOrDefaultAsync();

            if (duplicateAdmin != null)
            {
                return BadRequest("Provided email is belong to an admin!");
            }

            // check if email is in vendor collection
            var vendorFilter = Builders<Vendor>.Filter.Eq(ven => ven.Email, csr.Email);
            var duplicateVendor = await _vendors.Find(vendorFilter).FirstOrDefaultAsync();

            if (duplicateVendor != null)
            {
                return BadRequest("Provided email is belong to an vendor!");
            }

            // check if email is in csr collection
            var SalesRepFilter = Builders<CustomerSalesRep>.Filter.Eq(rep => rep.Email, csr.Email);
            var duplicateCSR = await _csrs.Find(SalesRepFilter).FirstOrDefaultAsync();

            if (duplicateCSR != null)
            {
                return BadRequest("Provided email is belong to an csr!");
            }

            csr.Id = null;
            csr.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(csr.NIC, 10);
            csr.Role = UserRoles.CSR;
            csr.Profile_Picture = UserProfiles.Profiles["CSR"];
            csr.First_Login = true;
            csr.Status = true;
            await _csrs.InsertOneAsync(csr);
            return CreatedAtAction(nameof(getSalesRepById), new { id = csr.Id }, csr);
        }

        // get CSR details by id
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

        //toggle account activation status
        [HttpPut("activate_csr/{id}")]
        public async Task<ActionResult> ToggleAccountActivation(string id)
        {
            var filter = Builders<CustomerSalesRep>.Filter.Eq(csr => csr.Id, id);
            var targetCSR = await _csrs.Find(filter).FirstOrDefaultAsync();

            if (targetCSR != null)
            {
                targetCSR.Status = !targetCSR.Status;
                await _csrs.ReplaceOneAsync(filter, targetCSR);
                return Ok();
            }

            return BadRequest("CSR account activation status can not be updated!");
        }

        // update the CSR password
        [HttpPut("{id}/update_password")]
        public async Task<ActionResult> UpdateVendorPassword(string id, [FromBody] string password)
        {
            var filter = Builders<CustomerSalesRep>.Filter.Eq(csr => csr.Id, id);
            var vendor = await _csrs.Find(filter).FirstOrDefaultAsync();

            if (vendor == null)
            {
                return NotFound("Sales rep not found.");
            }

            string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 10);
            var update = Builders<CustomerSalesRep>.Update
                .Set(csr => csr.Password, hashedPassword)
                .Set(csr => csr.First_Login, false);
            var result = await _csrs.UpdateOneAsync(filter, update);

            if (result.ModifiedCount > 0)
            {
                return Ok("CSR password updated successfully.");
            }

            return BadRequest("CSR password could not be updated.");
        }


        // delete a CSR account
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCSR(string id)
        {
            var filter = Builders<CustomerSalesRep>.Filter.Eq(csr => csr.Id, id);
            await _csrs.DeleteOneAsync(filter);
            return Ok();
        }
    }
}

// ---------------------------------------------------------------------------
// File: VendorController.cs
// Author: IT21189630
// Date Created: 2024-09-29
// Description: This file contains the logic for handling vendor management 
//              operations such as retrieving, adding, updating, and deleting vendor.
// Version: 1.0.0
// ---------------------------------------------------------------------------
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
        private readonly IMongoCollection<Admin> _admins;
        private readonly IMongoCollection<CustomerSalesRep> _csrs;

        public VendorController(MongoDBService mongoDBService) {
            _vendors = mongoDBService.database.GetCollection<Vendor>("vendors");
            _csrs = mongoDBService.database.GetCollection<CustomerSalesRep>("csrs");
            _admins = mongoDBService.database.GetCollection<Admin>("admins");
        }

        // get all vendors
        [HttpGet]
        public async Task<List<Vendor>> getVendors()
        {
            return await _vendors.Find(FilterDefinition<Vendor>.Empty).ToListAsync();
        }

        //create new vendor account
        [HttpPost]
        public async Task<ActionResult> createVendor(Vendor vendor)
        {
            // check if email is in admin collection
            var adminFilter = Builders<Admin>.Filter.Eq(adm => adm.Email, vendor.Email);
            var duplicateAdmin = await _admins.Find(adminFilter).FirstOrDefaultAsync();

            if (duplicateAdmin != null)
            {
                return BadRequest("Provided email is belong to an admin!");
            }

            // check if email is in vendor collection
            var vendorFilter = Builders<Vendor>.Filter.Eq(ven => ven.Email, vendor.Email);
            var duplicateVendor = await _vendors.Find(vendorFilter).FirstOrDefaultAsync();

            if (duplicateVendor != null)
            {
                return BadRequest("Provided email is belong to an vendor!");
            }

            // check if email is in csr collection
            var SalesRepFilter = Builders<CustomerSalesRep>.Filter.Eq(csr => csr.Email, vendor.Email);
            var duplicateCSR = await _csrs.Find(SalesRepFilter).FirstOrDefaultAsync();

            if (duplicateCSR != null)
            {
                return BadRequest("Provided email is belong to an csr!");
            }

            vendor.Id = null;
            vendor.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(vendor.NIC, 10);
            vendor.Role = UserRoles.VENDOR;
            vendor.Profile_Picture = UserProfiles.Profiles["Vendor"];
            vendor.First_Login = true;
            vendor.Status = true;
            await _vendors.InsertOneAsync(vendor);
            return CreatedAtAction(nameof(getVendorById), new { id = vendor.Id }, vendor);
        }

        // update the vendor using the id
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

        // update the vendor password
        [HttpPut("{id}/update_password")]
        public async Task<ActionResult> UpdateVendorPassword(string id, [FromBody] string password)
        {
            var filter = Builders<Vendor>.Filter.Eq(vendor => vendor.Id, id);
            var vendor = await _vendors.Find(filter).FirstOrDefaultAsync();

            if (vendor == null)
            {
                return NotFound("Vendor not found.");
            }

            string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 10);
            var update = Builders<Vendor>.Update
                .Set(vendor => vendor.Password, hashedPassword)
                .Set(vendor => vendor.First_Login, false);
            var result = await _vendors.UpdateOneAsync(filter, update);

            if (result.ModifiedCount > 0)
            {
                return Ok("Vendor password updated successfully.");
            }

            return BadRequest("Vendor password could not be updated.");
        }

        // delete a vendor
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVendor(string id)
        {
            var filter = Builders<Vendor>.Filter.Eq(vendor => vendor.Id, id);
            await _vendors.DeleteOneAsync(filter);
            return Ok();
        }
    }
}

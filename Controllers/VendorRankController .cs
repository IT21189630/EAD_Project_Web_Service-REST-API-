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
    public class VendorRankController : ControllerBase
    {
        private readonly IMongoCollection<VendorRank> _vendorranks;

        public VendorRankController(MongoDBService mongoDBService)
        {
            _vendorranks = mongoDBService.database.GetCollection<VendorRank>("vendorranks");

        }

        // GET: api/VendorRank/{vendorId}
        [HttpGet("{vendorId}")]
        public async Task<ActionResult<VendorRank>> GetVendorRankByVendorId(string vendorId)
        {
            // Query to find vendor rank by vendor_id
            var filter = Builders<VendorRank>.Filter.Eq(v => v.Vendor_Id, vendorId);

            var vendorRank = await _vendorranks.Find(filter).FirstOrDefaultAsync();

            if (vendorRank == null)
            {
                return NotFound($"Vendor with ID: {vendorId} not found.");
            }

            return Ok(vendorRank);
        }


    }
}

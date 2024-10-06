using EAD_Web_Service_API.Data;
using EAD_Web_Service_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EAD_Web_Service_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderCancellationController : ControllerBase
    {
        private readonly IMongoCollection<CancellationRequest> _cancellations;

        public OrderCancellationController(MongoDBService mongoDBService)
        {
            _cancellations = mongoDBService.database.GetCollection<CancellationRequest>("cancellations");
        }

        //get all cancellation requests
        [HttpGet]
        public async Task<IEnumerable<CancellationRequest>> GetAllRequests()
        {
            return await _cancellations.Find(FilterDefinition<CancellationRequest>.Empty).ToListAsync();
        }

        //create a cancellation request
        [HttpPost]
        public async Task<ActionResult<CancellationRequest>> CreateCancellationRequest(CancellationRequest cr)
        {
            cr.Id = null;
            cr.Status = "pending";
            cr.Created_At = DateTime.UtcNow;
            await _cancellations.InsertOneAsync(cr);
            return CreatedAtAction(nameof(GetCancellationRequestById), new { id = cr.Id }, cr);
        }

        // get a cancellation request by id
        [HttpGet("{id}")]
        public async Task<ActionResult<CancellationRequest>> GetCancellationRequestById(string id)
        {
            var filter = Builders<CancellationRequest>.Filter.Eq(request => request.Id, id);
            var cancellationRequest = await _cancellations.Find(filter).FirstOrDefaultAsync();

            if (cancellationRequest != null)
            {
                return Ok(cancellationRequest);
            }

            return NotFound();
        }

        // update cancellation request state when solving
        [HttpPut("solve/{id}")]
        public async Task<ActionResult> UpdateCancellation(string id)
        {
            var filter = Builders<CancellationRequest>.Filter.Eq(request => request.Id, id);
            var cancellation = await _cancellations.Find(filter).FirstOrDefaultAsync();

            if (cancellation != null)
            {
                cancellation.Status = "solved";
                await _cancellations.ReplaceOneAsync(filter, cancellation);

                return Ok();
            }

            return BadRequest("Product state can not be updated!");
        }

        // delete an cancellation request using the id
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCancellation(string id)
        {
            var filter = Builders<CancellationRequest>.Filter.Eq(request => request.Id, id);
            await _cancellations.DeleteOneAsync(filter);
            return Ok();
        }

    }
}

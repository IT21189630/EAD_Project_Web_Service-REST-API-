using EAD_Web_Service_API.Data;
using EAD_Web_Service_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EAD_Web_Service_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<Notification> _notifications;

        public ProductController(MongoDBService mongoDBService) {
            _products = mongoDBService.database.GetCollection<Product>("products");
            _notifications = mongoDBService.database.GetCollection<Notification>("notifications");
        }

        // get all products
        [HttpGet]
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _products.Find(FilterDefinition<Product>.Empty).ToListAsync();
        }

        // get all products available to customers
        [HttpGet("available")]
        public async Task<ActionResult<List<Product>>> GetAvailableProducts()
        {
            var filter = Builders<Product>.Filter.Eq(product => product.Visibility, true);
            var availableProducts = await _products.Find(filter).ToListAsync();

            if (availableProducts.Count > 0)
            {
                return Ok(availableProducts);
            }

            return NotFound("No available products found.");
        }

        // get all products related to vendor
        [HttpGet("vendor/{id}")]
        public async Task<ActionResult<List<Product>>> GetProductsByVendor(string id)
        {
            var filter = Builders<Product>.Filter.Eq(product => product.Vendor_Id, id);
            var products = await _products.Find(filter).ToListAsync();

            if (products.Count > 0)
            {
                return Ok(products);
            }

            return NotFound("No products created yet.");
        }


        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            await _products.InsertOneAsync(product);
            return Ok(product);
        }

        //get product by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var filter = Builders<Product>.Filter.Eq(product => product.Id, id);
            var product = await _products.Find(filter).FirstOrDefaultAsync();

            if(product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        //send notification method
        private async Task SendNotification(string receiverId, string productName, bool status)
        {
            string visbility = status ? "Available" : "Unavailable";

            var notification = new Notification
            {
                Receiver_Id = receiverId,
                Subject = "Product Visibility Changed!",
                Body = $"Administrator has changed the visibility of your product '{productName}' into {visbility}. You can see more information from your dashboard!",
                Viewed = false,
                Created_At = DateTime.UtcNow,
            };

            await _notifications.InsertOneAsync(notification);
        }

        //change product visibility
        [HttpPut("toggle_availability/{id}")]
        public async Task<ActionResult> UpdateProductAvailability(string id)
        {
            var filter = Builders<Product>.Filter.Eq(product => product.Id, id);
            var product = await _products.Find(filter).FirstOrDefaultAsync();

            if (product != null) { 
                product.Visibility = !product.Visibility;
                await _products.ReplaceOneAsync(filter, product);

                await SendNotification(product.Vendor_Id, product.Product_Name, product.Visibility);

                return Ok();
            }

            return BadRequest("Product state can not be updated!");
        }

        // update a product
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(Product product, string id)
        {
            var filter = Builders<Product>.Filter.Eq(prod => prod.Id, id);
            await _products.ReplaceOneAsync(filter, product);
            return Ok();
        }

        // delete a product
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            var filter = Builders<Product>.Filter.Eq(product => product.Id, id);
            await _products.DeleteOneAsync(filter);
            return Ok();
        }
    }
}

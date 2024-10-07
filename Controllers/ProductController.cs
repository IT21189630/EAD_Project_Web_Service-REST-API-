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

        //dispatch orders and update product remaining stong
        [HttpPut("update_stock/{id}")]
        public async Task<ActionResult> UpdateProductRemainingStock(string id, [FromBody] int qty)
        {
            var filter = Builders<Product>.Filter.Eq(product => product.Id, id);
            var product = await _products.Find(filter).FirstOrDefaultAsync();

            if (product != null)
            {
                //check available quantity is greater than requested quantity
                if (product.Stock > qty) {
                    product.Stock = product.Stock - qty;
                    await _products.ReplaceOneAsync(filter, product);
                    return Ok();
                }
                else
                {
                    return BadRequest("Not enough stock remaining");
                }

            }
            return BadRequest("Product state can not be updated!");
        }

        // update a product
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(Product product, string id)
        {
            var filter = Builders<Product>.Filter.Eq(prod => prod.Id, id);
            var update = Builders<Product>.Update
                .Set(prod => prod.Product_Name, product.Product_Name)
                .Set(prod => prod.Product_Category, product.Product_Category)
                .Set(prod => prod.Image, product.Image)
                .Set(prod => prod.Price, product.Price)
                .Set(prod => prod.Stock, product.Stock);
               
            var result = await _products.UpdateOneAsync(filter, update);

            if (result.ModifiedCount > 0)
            {
                return Ok();
            }
            else
            {
                return NotFound("Product not found or no changes made.");
            }
        }

        // delete a product
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            var filter = Builders<Product>.Filter.Eq(product => product.Id, id);
            await _products.DeleteOneAsync(filter);
            return Ok();
        }

        //get products by category
        [HttpGet("category/{category_name}")]
        public async Task<ActionResult<List<Product>>> GetProductsByCategory(string category_name)
        {
            var filter = Builders<Product>.Filter.Eq(product => product.Product_Category, category_name);
            var products = await _products.Find(filter).ToListAsync();

            if (products.Count > 0)
            {
                return Ok(products);
            }

            return NotFound("No products created yet.");
        }
    }
}

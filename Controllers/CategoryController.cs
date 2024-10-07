using EAD_Web_Service_API.Data;
using EAD_Web_Service_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EAD_Web_Service_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMongoCollection<Category> _categories;

        public CategoryController(MongoDBService mongoDBService)
        {
            _categories = mongoDBService.database.GetCollection<Category>("categories");
        }

        // get all categories
        [HttpGet]
        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _categories.Find(FilterDefinition<Category>.Empty).ToListAsync();
        }



        [HttpPost]
        public async Task<ActionResult<Category>> CreateProduct(Category category)
        {
            await _categories.InsertOneAsync(category);
            return Ok(category);
        }

    }
}


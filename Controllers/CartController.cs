// ---------------------------------------------------------------------------
// File: CartController.cs
// Author: IT21211164
// Date Created: 2024-09-30
// Description: This file contains the logic for handling cart management 
//              operations such as retrieving, adding, updating, and deleting cartItem.
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
    public class CartController : ControllerBase
    {
        private readonly IMongoCollection<Cart> _cartItems;

        public CartController(MongoDBService mongoDBService) {
            _cartItems = mongoDBService.database.GetCollection<Cart>("cartItems");
        }

        // get all cartItems
        [HttpGet]
        public async Task<IEnumerable<Cart>> GetAllCartItems()
        {
            return await _cartItems.Find(FilterDefinition<Cart>.Empty).ToListAsync();
        }


        //create cart item
        [HttpPost]
        public async Task<ActionResult<Cart>> CreateCartItem(Cart cartItem)
        {
            await _cartItems.InsertOneAsync(cartItem);
            return Ok(cartItem);
        }

 

        // update a cart item
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCartItem(Cart cartItem, string id)
        {
            var filter = Builders<Cart>.Filter.Eq(prod => prod.Id, id);
            var update = Builders<Cart>.Update
                .Set(prod => prod.Product_Name, cartItem.Product_Name)
                .Set(prod => prod.Price, cartItem.Price)
                .Set(prod => prod.Number_Of_Items, cartItem.Number_Of_Items)
                .Set(prod => prod.Image, cartItem.Image);

               
            var result = await _cartItems.UpdateOneAsync(filter, update);

            if (result.ModifiedCount > 0)
            {
                return Ok();
            }
            else
            {
                return NotFound("CartItem not found or no changes made.");
            }
        }

        // delete a cart item
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCartItem(string id)
        {
            var filter = Builders<Cart>.Filter.Eq(cartItem => cartItem.Id, id);
            await _cartItems.DeleteOneAsync(filter);
            return Ok();
        }
    }
}

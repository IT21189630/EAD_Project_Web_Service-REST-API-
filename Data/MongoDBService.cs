// ---------------------------------------------------------------------------
// File: MongoDBService.cs
// Date Created: 2024-09-29
// Description: This file contains the logic for establishing database connection with mongo database.
// Version: 1.0.0
// ---------------------------------------------------------------------------
using MongoDB.Driver;

namespace EAD_Web_Service_API.Data
{
    public class MongoDBService
    {
        // below code was referenced by a youtube tutorial
        // video name: MongoDB with DotNet - .Net Rest API Tutorial | MongoDB
        // author name: Coding Droplets
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase _database;

        public MongoDBService(IConfiguration configuration) { 
            _configuration = configuration;
            var connection_string = _configuration.GetConnectionString("dbConnection");
            var mongoUrl = MongoUrl.Create(connection_string);
            var mongoClient = new MongoClient(mongoUrl);
            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoDatabase database => _database;
    }
}

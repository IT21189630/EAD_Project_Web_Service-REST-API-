using MongoDB.Driver;

namespace EAD_Web_Service_API.Data
{
    public class MongoDBService
    {
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

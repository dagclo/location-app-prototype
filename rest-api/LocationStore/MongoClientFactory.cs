using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace rest_api.LocationStore
{
    public class MongoClientFactory : IMongoClientFactory
    {
        private readonly MongoDBLocationStoreConfiguration _configuration;
        private readonly ILogger<MongoClientFactory> _logger;
        private string _indexName;

        public MongoClientFactory(MongoDBLocationStoreConfiguration configuration, ILogger<MongoClientFactory> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string Create2dSphereIndex()
        {
            //TODO: check if index exists before creating it
            //var indexEnumerator = collection.Indexes.ListAsync().GetAwaiter().GetResult();
            //bool foundIndex = false;
            //while (indexEnumerator.MoveNext())
            //{
            //    var doc = indexEnumerator.Current;
            //    foreach(var index in doc)
            //    {
            //        if(index.)
            //    }
            //}

            var keys = Builders<MongoDBLocationDocument>.IndexKeys.Geo2DSphere(l => l.Coordinates);
            var model = new CreateIndexModel<MongoDBLocationDocument>(keys);
            var collection = GetCollection();
            _indexName = collection.Indexes.CreateOne(model);
            return _indexName;
        }

        public IMongoClient GetClient()
        {
            //TODO: use a user from a specific db instead of the admin user
            _logger.LogInformation("connecting with {connection_string} and creds {username}", _configuration.MONGO_DB_CONNECTION, _configuration.MONGO_INITDB_ROOT_USERNAME);
            var credential = MongoCredential.CreateCredential(_configuration.MONGO_DB_NAME, _configuration.MONGO_INITDB_ROOT_USERNAME, _configuration.MONGO_INITDB_ROOT_PASSWORD);
            var settings = MongoClientSettings.FromConnectionString(_configuration.MONGO_DB_CONNECTION);
            settings.Credential = credential;
            return new MongoClient(settings);
        }

        public IMongoCollection<MongoDBLocationDocument> GetCollection()
        {
            var client = GetClient();
            return client.GetDatabase(_configuration.MONGO_DB_NAME).GetCollection<MongoDBLocationDocument>(_configuration.MONGO_DB_COLLECTION_NAME);
        }
    }
}

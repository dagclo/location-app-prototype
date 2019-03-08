using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using rest_api.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rest_api.LocationStore
{
    public class MongoDBLocationStoreConfiguration
    {
        public string MONGO_DB_CONNECTION { get; set; }
        public string MONGO_DB_NAME { get; set; }
        public string MONGO_DB_COLLECTION_NAME { get; set; }

    }

    public class MongoDBLocationDocument
    {
        public string Id { get; set; }

        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Coordinates { get; set; }
    }

    public class MongoDBLocationStore : ILocationStore
    {
        private readonly string _connection;
        private readonly string _dbName;
        private readonly string _collectionName;
        private readonly string _indexName;
        private const string _locationIndexName = "locationIndex";

        public MongoDBLocationStore(MongoDBLocationStoreConfiguration configuration)
        {
            _connection = configuration.MONGO_DB_CONNECTION;
            _dbName = configuration.MONGO_DB_NAME;
            _collectionName = configuration.MONGO_DB_COLLECTION_NAME;
            var collection = GetCollection();

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
            _indexName = collection.Indexes.CreateOne(model);
            
        }

        private IMongoCollection<MongoDBLocationDocument> GetCollection()
        {
            var mongoClient = new MongoClient(_connection);
            var db = mongoClient.GetDatabase(_dbName);
            
            var collection = db.GetCollection<MongoDBLocationDocument>(_collectionName);
            
            return collection;
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Location> GetAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Location>> SearchAsync(double latitude, double longitude, double radiusInMiles)
        {
            throw new NotImplementedException();
        }

        public Task<string> StoreAsync(Location location)
        {
            throw new NotImplementedException();
        }
    }
}

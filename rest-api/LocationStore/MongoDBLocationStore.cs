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
        public string MONGO_INITDB_ROOT_USERNAME { get; set; }
        public string MONGO_INITDB_ROOT_PASSWORD { get; set; }

    }

    public class MongoDBLocationDocument
    {
        public MongoDBLocationDocument(string id, string text, double longitude, double latitude)
        {
            Id = id;
            Text = text;
            this.Coordinates = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(longitude, latitude));
        }

        public string Id { get; set; }

        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Coordinates { get; set; }
        public string Text { get; set; }

        public Location ToLocation()
        {
            return new Location(Id, Text, Coordinates.Coordinates.Latitude, Coordinates.Coordinates.Longitude);
        }
    }

    public interface IMongoClientFactory
    {
        IMongoClient GetClient();

        IMongoCollection<MongoDBLocationDocument> GetCollection();

        string Create2dSphereIndex();
    }

    public class MongoClientFactory : IMongoClientFactory
    {
        private readonly MongoDBLocationStoreConfiguration _configuration;
        private string _indexName;

        public MongoClientFactory(MongoDBLocationStoreConfiguration configuration)
        {
            _configuration = configuration;
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
            return new MongoClient(_configuration.MONGO_DB_CONNECTION);
        }

        public IMongoCollection<MongoDBLocationDocument> GetCollection()
        {
            var client = GetClient();
            return client.GetDatabase(_configuration.MONGO_DB_NAME).GetCollection<MongoDBLocationDocument>(_configuration.MONGO_DB_COLLECTION_NAME);
        }
    }

    public class MongoDBLocationStore : ILocationStore
    {
        private readonly IMongoClientFactory _factory;
        private FilterDefinition<MongoDBLocationDocument> CreateIdFilter(string id)
        {
            return Builders<MongoDBLocationDocument>.Filter.Eq<string>(d => d.Id, id);
        }

        public MongoDBLocationStore(IMongoClientFactory factory)
        {
            _factory = factory;
            _factory.Create2dSphereIndex();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var collection = _factory.GetCollection();
            var result =await collection.DeleteOneAsync(CreateIdFilter(id));
            return result.IsAcknowledged;
        }

        public async Task<Location> GetAsync(string id)
        {
            var collection = _factory.GetCollection();
            var doc = await collection.FindAsync(CreateIdFilter(id));
            return doc.FirstOrDefault()?.ToLocation();
        }

        public async Task<IEnumerable<Location>> SearchAsync(double latitude, double longitude, double radiusInMiles, int limit)
        {
            var collection = _factory.GetCollection();
            var point = GeoJson.Point(GeoJson.Geographic(longitude, latitude));
            var locFilter = Builders<MongoDBLocationDocument>.Filter.Near(x => x.Coordinates, point, radiusInMiles.ToMeters());
            var query = collection.Find(locFilter).Limit(limit);
            return (await query.ToListAsync()).Select(d => d.ToLocation());
        }

        public async Task<string> StoreAsync(Location location)
        {
            var collection = _factory.GetCollection();
            await collection.InsertOneAsync(location.ToDocument());
            return location.Id;
        }
    }

    public static class DistanceConversions
    {
        public static double ToMeters(this double miles)
        {
            return miles * 1609.34;
        }
    }
}

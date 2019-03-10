using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using rest_api.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rest_api.LocationStore
{

    public class MongoDBLocationStore : ILocationStore
    {
        private readonly IMongoClientFactory _factory;
        private readonly ILogger<MongoDBLocationStore> _logger;

        private FilterDefinition<MongoDBLocationDocument> CreateIdFilter(string id)
        {
            return Builders<MongoDBLocationDocument>.Filter.Eq<string>(d => d.Id, id);
        }

        public MongoDBLocationStore(IMongoClientFactory factory, ILogger<MongoDBLocationStore> logger)
        {
            _factory = factory;
            _factory.Create2dSphereIndex();
            _logger = logger;
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
            var document = location.ToDocument();
            document.Id = Guid.NewGuid().ToString();
            _logger.LogInformation("creating document {id}", document.Id);
            await collection.InsertOneAsync(document);
            return document.Id;
        }
    }
}

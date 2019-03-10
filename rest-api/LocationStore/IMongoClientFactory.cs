using MongoDB.Driver;

namespace rest_api.LocationStore
{
    public interface IMongoClientFactory
    {
        IMongoClient GetClient();

        IMongoCollection<MongoDBLocationDocument> GetCollection();

        string Create2dSphereIndex();
    }
}

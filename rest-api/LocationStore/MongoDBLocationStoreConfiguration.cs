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
}

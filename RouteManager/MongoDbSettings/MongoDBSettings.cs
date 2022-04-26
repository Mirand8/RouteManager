namespace RouteManager.MongoDbSettings
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string ExcelFileCollectionName { get; set; }
        public string DatabaseName { get; set; }
    }
}

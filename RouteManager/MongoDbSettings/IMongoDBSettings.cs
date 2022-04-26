namespace RouteManager.MongoDbSettings
{
    public interface IMongoDBSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string ExcelFileCollectionName { get; set; }
    }
}

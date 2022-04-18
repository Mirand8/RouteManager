namespace PersonApiService.Settings
{
    public interface IPersonApiServiceSettings
    {
        string ConnectionString { get; set; }
        string PersonCollectionName { get; set; }
        string DatabaseName { get; set; }
    }
}
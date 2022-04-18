namespace PersonApiService.Settings
{
    public class PersonApiServiceSettings : IPersonApiServiceSettings
    {
        public string ConnectionString { get; set; }
        public string PersonCollectionName { get; set; }
        public string DatabaseName { get; set; }
    }
}

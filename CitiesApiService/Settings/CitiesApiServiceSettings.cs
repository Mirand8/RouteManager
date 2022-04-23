namespace CitiesApiService.Settings
{
    public class CitiesApiServiceSettings : ICitiesApiServiceSettings
    {
        public string ConnectionString { get; set; }
        public string CityCollectionName { get; set; }
        public string DatabaseName { get; set; }
    }
}

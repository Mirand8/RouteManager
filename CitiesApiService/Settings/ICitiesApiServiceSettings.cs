namespace CitiesApiService.Settings
{
    public interface ICitiesApiServiceSettings
    {
        string ConnectionString { get; set; }
        string CityCollectionName { get; set; }
        string DatabaseName { get; set; }
    }
}
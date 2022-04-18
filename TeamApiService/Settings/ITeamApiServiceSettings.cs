namespace TeamApiService.Settings
{
    public interface ITeamApiServiceSettings
    {
        string ConnectionString { get; set; }
        string TeamCollectionName { get; set; }
        string DatabaseName { get; set; }
    }
}

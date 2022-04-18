namespace TeamApiService.Settings
{
    public class TeamApiServiceSettings : ITeamApiServiceSettings
    {
        public string ConnectionString { get; set; }
        public string TeamCollectionName { get; set; }
        public string DatabaseName { get; set; }
    }
}

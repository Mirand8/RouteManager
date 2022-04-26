namespace ExcelFileApiService.Settings
{
    public class ExcelFileApiServiceSettings : IExcelFileApiServiceSettings
    {
        public string ConnectionString { get; set; }
        public string Databasename { get; set; }
        public string ExcelFileCollectionName { get; set; }
    }
}

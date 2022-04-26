namespace ExcelFileApiService.Settings
{
    public interface IExcelFileApiServiceSettings
    {
        string ConnectionString { get; set; }
        string Databasename { get; set; }
        string ExcelFileCollectionName { get; set; }
    }
}

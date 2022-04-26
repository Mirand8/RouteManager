using MongoDB.Driver;
using RouteManager.Models;
using RouteManager.MongoDbSettings;
using System.Collections.Generic;
using System.Linq;

namespace RouteManager.Services
{
    public class ExcelFileService
    {
        readonly IMongoCollection<ExcelFile> _excelFileService;

        public ExcelFileService(IMongoDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _excelFileService = database.GetCollection<ExcelFile>(settings.ExcelFileCollectionName);
        }

        public ExcelFile Get() => _excelFileService.Find(excel => true).FirstOrDefault();
        public void Create(ExcelFile excel) => _excelFileService.InsertOne(excel);
        public void Remove() => _excelFileService.DeleteOne(x => true);
        public IEnumerable<string> GetServices() => _excelFileService.Find(excel => true)
                                                                    .FirstOrDefault().ExcelFiles
                                                                    .Select(x => x.GetValue("SERVIÇO").ToString())
                                                                    .Distinct();
        public IEnumerable<string> GetCities() => _excelFileService.Find(excel => true)
                                                                    .FirstOrDefault().ExcelFiles
                                                                    .Select(x => x.GetValue("CIDADE").ToString())
                                                                    .Distinct();
        public IEnumerable<string> GetCitiesByServices(string servico) => _excelFileService.Find(excel => true)
                                                                                           .FirstOrDefault().ExcelFiles
                                                                                           .Where(x => x.GetValue("SERVIÇO").ToString() == servico)
                                                                                           .Select(x => x.GetValue("CIDADE").ToString());
        public IEnumerable<string> GetServicesByCities(string cidade) => _excelFileService.Find(excel => true)
                                                                                        .FirstOrDefault().ExcelFiles
                                                                                        .Where(x => x.GetValue("CIDADE").ToString() == cidade)
                                                                                        .Select(x => x.GetValue("SERVIÇO").ToString());
    }
}

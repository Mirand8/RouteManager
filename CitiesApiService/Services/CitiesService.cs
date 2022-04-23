using CitiesApiService.Settings;
using ModelsLib;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CitiesApiService.Services
{
    public class CitiesService
    {
        readonly IMongoCollection<City> _cities;

        public CitiesService(ICitiesApiServiceSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _cities = database.GetCollection<City>(settings.CityCollectionName);
        }

        public async Task<IEnumerable<City>> Get() =>
            await _cities.Find(city => true)
                         .SortBy(city => city.Name)
                         .SortBy(city => city.State)
                         .ToListAsync();

        public async Task<City> GetById(string id) =>
            await _cities.Find(city => city.Id == id)
                         .FirstOrDefaultAsync<City>();

        public async Task<City> GetByName(string name) =>
            await _cities.Find(city => city.Name.ToLower() == name.ToLower())
                         .FirstOrDefaultAsync<City>();

        public async Task<IEnumerable<City>> GetCitiesByState(string state) =>
           await _cities.Find(city => city.State.ToLower() == state.ToLower())
                        .SortBy(city => city.Name)
                        .ToListAsync();

        public async Task<City> Create(City city)
        {
            await _cities.InsertOneAsync(city);
            return city;
        }

        public async Task<City> Update(string id, City cityParam)
        {
            var city = await GetById(id);

            if (city == null) return null;
            await _cities.ReplaceOneAsync(city => city.Id == id, cityParam);

            return cityParam;
        }

        public async Task<City> Remove(string id)
        {
            var city = await GetById(id);

            if (city == null) return null;
            await _cities.DeleteOneAsync(city => city.Id == id);

            return city;
        }
    }
}

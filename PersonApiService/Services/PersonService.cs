using ModelsLib;
using MongoDB.Driver;
using PersonApiService.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonApiService.Services
{
    public class PersonService
    {
        readonly IMongoCollection<Person> _people;

        public PersonService(IPersonApiServiceSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _people = database.GetCollection<Person>(settings.PersonCollectionName);
        }


        public async Task<IEnumerable<Person>> Get() =>
            await _people.Find(person => true)
                         .SortBy(person => person.Name)
                         .ToListAsync();


        public async Task<Person> Get(string id) =>
            await _people.Find(person => person.Id == id)
                         .FirstOrDefaultAsync();


        public async Task<IEnumerable<Person>> GetByName(string name) =>
            await _people.Find(person => person.Name.ToLower().Contains(name.ToLower()))
                         .SortBy(person => person.Name)
                         .ToListAsync();


        public async Task<Person> Create(Person person)
        {
            await _people.InsertOneAsync(person);
            return person;
        }


        public async Task<Person> Update(string id, Person personParam)
        {
            var person = await Get(id);
            if (person == null) return null;

            await _people.ReplaceOneAsync<Person>(person => person.Id == id, personParam);
            return personParam;
        }


        public async Task<Person> UpdateAvailablety(string id)
        {
            var person = await Get(id);
            if (person == null) return null;

            person.IsAvailableToTeam = !person.IsAvailableToTeam;
            await _people.ReplaceOneAsync(person => person.Id == id, person);
            return person;
        }


        public async Task<Person> Delete(string id)
        {
            var person = await Get(id);
            if (person == null) return null;

            await _people.DeleteOneAsync<Person>(person => person.Id == id);
            return person;
        }


    }
}

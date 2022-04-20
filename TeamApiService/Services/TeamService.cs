using ModelsLib;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamApiService.Settings;

namespace TeamApiService.Services
{
    public class TeamService
    {
        readonly IMongoCollection<Team> _teams;

        public TeamService(ITeamApiServiceSettings settings)
        {
            var team = new MongoClient(settings.ConnectionString);
            var database = team.GetDatabase(settings.DatabaseName);
            _teams = database.GetCollection<Team>(settings.TeamCollectionName);
        }

        public async Task<IEnumerable<Team>> Get() =>
           await _teams.Find(team => true)
                       .SortBy(team => team.Name)
                       .ToListAsync();

        public async Task<Team> Get(string id) =>
           await _teams.Find(team => team.Id == id)
                       .FirstOrDefaultAsync<Team>();

        public async Task<Team> GetByName(string name) =>
            await _teams.Find(team => team.Name.ToLower() == name.ToLower())
                        .FirstOrDefaultAsync<Team>();

        public async Task<Team> Create(Team teamParam)
        {
            foreach (var person in teamParam.Members)
            {
                await PersonService.UpdateAvailablety(person.Id);
                person.IsAvailableToTeam = !person.IsAvailableToTeam;
            }

            await _teams.InsertOneAsync(teamParam);

            return teamParam;
        }

        public async Task<Team> Update(string id, Team teamParam)
        {
            var team = await Get(id) ?? null;
            await _teams.ReplaceOneAsync(team => team.Id == id, teamParam);
            return team;
        }

        public async Task<Team> UpdateAvailablety(string id)
        {
            var team = await Get(id) ?? null;

            team.IsAvailable = !team.IsAvailable;

            await _teams.ReplaceOneAsync(team => team.Id == id, team);

            return team;
        }

        public async Task<Team> UpdateToInsert(string id, Person personParam)
        {
            var team = await Get(id) ?? null;

            personParam.IsAvailableToTeam = false;
            var filter = Builders<Team>.Filter.Where(team => team.Id == id);
            var update = Builders<Team>.Update.Push("Members", personParam);

            await PersonService.UpdateAvailablety(personParam.Id);
            await _teams.UpdateOneAsync(filter, update);
            return team;
        }

        public async Task<Team> UpdateToDelete(string id, Person personParam)
        {
            var team = await Get(id) ?? null;

            var filter = Builders<Team>.Filter.Where(team => team.Id == id);
            var update = Builders<Team>.Update.Pull("Members", personParam);

            await PersonService.UpdateAvailablety(personParam.Id);
            await _teams.UpdateOneAsync(filter, update);
            return team;
        }

        public async Task<Team> Delete(string id)
        {
            var team = await Get(id) ?? null;

            foreach (var person in team.Members) await PersonService.UpdateAvailablety(person.Id);

            await _teams.DeleteOneAsync(team => team.Id == id);
            return team;
        }

    }
}

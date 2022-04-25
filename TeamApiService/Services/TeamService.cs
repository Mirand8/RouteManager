using ModelsLib;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
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
                       .ToListAsync();

        public async Task<Team> Get(string id) =>
           await _teams.Find(team => team.Id == id)
                       .FirstOrDefaultAsync<Team>();

        public async Task<Team> GetByName(string name) =>
            await _teams.Find(team => team.Name == name)
                        .FirstOrDefaultAsync<Team>();

        public async Task<IEnumerable<Team>> GetByCity(string cityId) =>
            await _teams.Find(team => team.City.Id == cityId)
                        .SortBy(team => team.Name)
                        .ToListAsync();

        public async Task<Team> Create(Team teamParam)
        {
            await _teams.InsertOneAsync(teamParam); 
            return teamParam;
        }

        public async Task<Team> Update(string id, Team teamParam)
        {
            var team = await Get(id) ?? null;
            await _teams.ReplaceOneAsync(team => team.Id == id, teamParam);
            return team;
        }

        public async Task<Team> Delete(string id)
        {
            var team = await Get(id) ?? null;
            await _teams.DeleteOneAsync(team => team.Id == id);
            return team;
        }

        

        public async Task<Team> UpdateAvailablety(string id)
        {
            var team = await Get(id) ?? null;
            team.IsAvailable = !team.IsAvailable;
            await _teams.ReplaceOneAsync(team => team.Id == id, team);

            return team;
        }

        public async Task<Team> UpdateToAddMember(string id, Person personParam)
        {
            var team = await Get(id) ?? null;
            if (team == null) return team;

            personParam.CurrentTeam = team.Name;
            var filter = Builders<Team>.Filter.Where(x => x.Id == team.Id);
            var update = Builders<Team>.Update.Push("Members", personParam);
            await _teams.UpdateOneAsync(filter, update);

            await PersonService.UpdateCurrentTeam(personParam.Id, team.Name);

            return team;
        }

        public async Task<Team> UpdateToRemoveMember(string id, Person personParam)
        {
            var team = await Get(id) ?? null;
            if (team == null) return team;

            var personQuery = await PersonService.Get(personParam.Id);
            if (personQuery != null)
            {
                var filter = Builders<Team>.Filter.Where(x => x.Id == team.Id);
                var pullMemberDefinition = Builders<Team>.Update.PullFilter(x => x.Members, member => member.Id == personQuery.Id);
                await _teams.UpdateOneAsync(filter, pullMemberDefinition);
                await PersonService.UpdateCurrentTeam(personQuery.Id, null);
            }

            team.Members.Remove(team.Members.Find(x => x.Id == personParam.Id));
            return team;
        }

        public async Task<Team> UpdateMemberName(string personId, string newPersonName)
        {
            var person = await PersonService.Get(personId);
            if (person == null) return null;

            var personTeam = await GetByName(person.CurrentTeam);
            if (personTeam == null) return null;

            var filter = Builders<Team>.Filter.Where(team => team.Id == personTeam.Id && team.Members.Any(member => member.Id.Equals(personId)));
            var update = Builders<Team>.Update.Set(team => team.Members.ElementAt(-1).Name, newPersonName);

            await _teams.UpdateOneAsync(filter, update);
            return personTeam;
        }
    }
}

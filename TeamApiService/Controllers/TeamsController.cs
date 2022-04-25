using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsLib;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApiService.Services;

namespace TeamApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly TeamService _teamService;

        public TeamsController(TeamService teamsService)
        {
            _teamService = teamsService;
        }

        [HttpGet]
        public async Task<IEnumerable<Team>> Get() => await _teamService.Get();

        [HttpGet("{id:length(24)}", Name = "GetTeam")]
        public async Task<dynamic> Get(string id)
        {
            var team = await _teamService.Get(id);
            if (team == null) return NotFound("Time nao encontrado!");
            return team;
        }

        [HttpGet("{name}")]
        public async Task<dynamic> GetByName(string name)
        {
            var team = await _teamService.GetByName(name);
            if (team == null) return NotFound("Time nao encontrado!");
            return team;
        }

        [HttpGet("City/{cityId:length(24)}")]
        public async Task<IEnumerable<Team>> GetTeamsByCity([FromRoute] string cityId) =>
           await _teamService.GetByCity(cityId);

        [HttpPost]
        public async Task<dynamic> Post([FromBody] Team teamParam)
        {
            if (await _teamService.GetByName(teamParam.Name) != null) return BadRequest("Ja existe um time com esse nome!");

            var city = await CitiesService.Get(teamParam.City.Id);
            if (city == null) return BadRequest("Esta cidade não existe!");

            var people = new List<Person>();
            foreach (var person in teamParam.Members)
            {
                if (people.Any(x => x.Id == person.Id)) return BadRequest("Esta pessoa ja foi adicionada no time");
                var personQuery = await PersonService.Get(person.Id);
                if (personQuery == null) return NotFound($"Pessoa de {person.Id} nao encontrada");
                if (!string.IsNullOrEmpty(personQuery.CurrentTeam)) return BadRequest($"{personQuery.Name} ja esta em um time!");
                people.Add(personQuery);
            }
            people.ForEach(x => x.CurrentTeam = teamParam.Name);
            people.ForEach(async x => await PersonService.UpdateCurrentTeam(x.Id, teamParam.Name));

            teamParam.City = city;
            teamParam.Members = people;
            await _teamService.Create(teamParam);

            return CreatedAtRoute("GetTeam", new { id = teamParam.Id }, teamParam);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<dynamic> Update(string id, [FromBody] Team teamParam)
        {
            var team = await _teamService.Get(id);

            if (teamParam.City == null) return BadRequest("Time nao pode esta vazio");

            if (!team.Name.Equals(teamParam.Name))
            {
                if (await _teamService.GetByName(teamParam.Name) != null) return BadRequest($"O time de nome {teamParam.Name} ja existe!");
                if (teamParam.Name != team.Name)
                    team.Members.ForEach(async x => await PersonService.UpdateCurrentTeam(x.Id, teamParam.Name));
            }

            teamParam.Members = team.Members;

            if (!team.City.Id.Equals(teamParam.City.Id))
                teamParam.City = await CitiesService.Get(teamParam.City.Id);

            var response = await _teamService.Update(id, teamParam);
            if (response == null) return NotFound("Time nao encontrado!");

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<dynamic> Delete(string id)
        {
            var team = await _teamService.Delete(id);
            if (team == null) return NotFound("Time nao encontrado!");

            team.Members.ForEach(async member => await PersonService.UpdateCurrentTeam(member.Id, null));

            return NoContent();
        }

        [HttpPut("Availablety/{id:length(24)}")]
        public async Task<dynamic> UpdateAvailablety(string id)
        {
            var team = await _teamService.UpdateAvailablety(id);
            if (team == null) return NotFound("Time nao encontrado!");
            return NoContent();
        }

        [HttpPut("AddNewMember/{id:length(24)}")]
        public async Task<dynamic> UpdateToAddMember(string id, [FromBody] Person newMember)
        {
            var personQuery = await PersonService.Get(newMember.Id);
            if (personQuery == null) return BadRequest("Pessoa não encontrada!");

            if (!string.IsNullOrEmpty(personQuery.CurrentTeam))
                return BadRequest($"{personQuery.Name}:{personQuery.Id} ja esta em um time e deve ser removido do atual time ({personQuery.CurrentTeam}) antes de adiciona-lo em outro!");

            var team = await _teamService.UpdateToAddMember(id, personQuery);
            if (team == null) return NotFound("Time nao encontrado!");
            if (team.Name == "$DuplicatePersonError$") return BadRequest("A pessoa ja esta no time!");

            return NoContent();
        }

        [HttpPut("RemoveMember/{id:length(24)}")]
        public async Task<dynamic> UpdateToRemoveMember([FromRoute] string id, [FromBody] Person memberToRemove)
        {
            var team = await _teamService.UpdateToRemoveMember(id, memberToRemove);
            if (team == null) return NotFound("Time nao encontrado!");

            return NoContent();
        }

        [HttpPut("UpdateMemberName/{personId}/{newName}")]
        public async Task<dynamic> UpdateMemberName(string personId, string newName)
        {
            var team = await _teamService.UpdateMemberName(personId, newName);
            if (team == null) return NotFound("Time nao encontrado");

            return NoContent();
        }
    }
}

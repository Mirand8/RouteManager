using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsLib;
using System.Collections.Generic;
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


        [HttpPost]
        public async Task<dynamic> Post([FromBody] Team teamParam)
        {
            if (await _teamService.GetByName(teamParam.Name) != null) return BadRequest("Ja existe um time com esse nome!");
            await _teamService.Create(teamParam);
            return CreatedAtRoute("GetTeam", new { id = teamParam.Id }, teamParam);
        }


        [HttpPut("{id:length(24)}")]
        public async Task<dynamic> Update(string id, [FromBody] Team teamParam)
        {
            var team = await _teamService.Get(id);

            if (!team.Name.Equals(teamParam.Name))
                if (await _teamService.GetByName(teamParam.Name) != null) return BadRequest($"O time de nome {teamParam.Name} ja existe!");

            teamParam.Members = team.Members;
            team.Members.ForEach(async x => {
                var person = await PersonService.Get(x.Id);
                await PersonService.UpdateAvailablety(x.Id);
            });
            var response = await _teamService.Update(id, teamParam);
            if (response == null) return NotFound("Time nao encontrado!");

            return NoContent();
        }


        [HttpPut("{id:length(24)}/Status")]
        public async Task<dynamic> UpdateStatus(string id)
        {
            var team = await _teamService.UpdateAvailablety(id);
            if (team == null) return NotFound("Time nao encontrado!");
            return NoContent();
        }


        [HttpPut("{id:length(24)}/AddPerson")]
        public async Task<dynamic> UpdateInsert(string id, [FromBody] Person personParam)
        {
            var team = await _teamService.UpdateToInsert(id, personParam);
            if (team == null) return NotFound("Time nao encontrado!");
            return NoContent();
        }


        [HttpPut("{id:length(24)}/RemovePerson")]
        public async Task<dynamic> UpdateRemove(string id, [FromBody] Person personParam)
        {
            var team = await _teamService.UpdateToDelete(id, personParam);
            if (team == null) return NotFound("Time nao encontrado!");

            return NoContent();
        }


        [HttpDelete("{id:length(24)}")]
        public async Task<dynamic> Delete(string id)
        {
            var team = await _teamService.Delete(id);
            if (team == null) return NotFound("Time nao encontrado!");
            return NoContent();
        }
    }
}

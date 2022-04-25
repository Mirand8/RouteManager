using Microsoft.AspNetCore.Mvc;
using ModelsLib;
using PersonApiService.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        readonly PersonService _personService;
        

        public PeopleController(PersonService personService)
        {
            _personService = personService;
        }


        [HttpGet]
        public async Task<IEnumerable<Person>> Get() => await _personService.Get();


        [HttpGet("{id:length(24)}", Name = "GetPerson")]
        public async Task<ActionResult<Person>> Get(string id)
        {
            var person = await _personService.Get(id);
            if (person == null) return NotFound("Pessoa nao encontrada!");
            return Ok(person);
        }

        [HttpGet("{name}", Name = "GetPeopleByName")]
        public async Task<dynamic> GetByName(string name)
        {
            var person = await _personService.GetByName(name);
            if (person == null) return NotFound($"Pessoa de nome {name} nao encontrada");
            return person;
        }

        [HttpPost]
        public async Task<dynamic> Create([FromBody]Person personParam)
        {
            var person = await _personService.Create(personParam);
            return CreatedAtRoute("GetPerson", new { id = person.Id }, person);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<dynamic> Update(string id, [FromBody] Person personParam)
        {
            var person = await _personService.Update(id, personParam);

            if (person == null) return BadRequest("Pessoa nao encontrada");
            return NoContent();
        }

        [HttpPut("CurrentTeam/{id:length(24)}")]
        public async Task<dynamic> UpdateCurrentTeam (string id, string newTeam = null)
        {
            var person = await _personService.UpdateCurrentTeam(id, newTeam);
            if (person == null) return NotFound("Pessoa nao encontrada");

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<dynamic> Delete(string id)
        {
            var person = await _personService.Get(id);

            var personTeam = await TeamService.GetByName(person.CurrentTeam);
            if (personTeam == null) return NotFound($"Time {person.CurrentTeam} não encontrado!");
            await TeamService.UpdateToRemoveMember(personTeam.Id, person);

            person = await _personService.Delete(id);
            if (person == null) return NotFound("Pessoa nao encontrada!");
            
            return NoContent();
        }

    }
}

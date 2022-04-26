using CitiesApiService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsLib;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitiesApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly CitiesService _citiesService;

        public CitiesController(CitiesService citiesService) => _citiesService = citiesService;


        [HttpGet]
        public async Task<IEnumerable<City>> Get() =>
            await _citiesService.Get();

        [HttpGet("{id:length(24)}", Name = "GetCity")]
        public async Task<ActionResult<dynamic>> Get(string id)
        {
            var city = await _citiesService.Get(id);
            if (city == null) return NotFound("Cidade nao encontrada!");

            return city;
        }

        [HttpGet("{name}")]
        public async Task<dynamic> GetByName(string name)
        {
            var city = await _citiesService.GetByName(name);
            if (city == null) return NotFound("Cidade nao encontrada!");

            return city;
        }

        [HttpGet("ByState/{state}")]
        public async Task<dynamic> GetCitiesByState(string state)
        {
            var cities = await _citiesService.GetCitiesByState(state);
            if (cities == null) return NotFound("Cidade nao encontrada!");

            return cities;
        }

        [HttpPost]
        public async Task<dynamic> Post([FromBody] City cityParam)
        {
            var cities = await _citiesService.Get();
            if (cities.Any(x => x.Name == cityParam.Name)) return BadRequest("Ja existe uma cidade com esse nome!");

            cityParam.Name = cityParam.Name.ToUpper();
            cityParam.State = cityParam.State.ToUpper();
            var city = await _citiesService.Create(cityParam);

            return CreatedAtRoute("GetCity", new { id = city.Id }, city);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<dynamic> Update(string id, [FromBody] City cityParam)
        {
            var cities = await _citiesService.Get();
            if (cities.Any(x => x.Name.ToLower() == cityParam.Name.ToLower())) return BadRequest("Ja existe uma cidade com esse nome!");

            var city = await _citiesService.Get(id);
            var teamsInCity = await TeamService.GetTeamsByCity(city.Id);
            if (teamsInCity != null && cityParam.Name != city.Name)
            {
                teamsInCity.ToList().ForEach(x => x.City = cityParam);
                foreach (var team in teamsInCity)
                {
                    TeamService.Update(team.Id, team.City);
                }
            }

            city = await _citiesService.Update(id, cityParam);
            if (city == null) return NotFound("Cidade nao encontrada!");
            
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<dynamic> Delete(string id)
        {
            var city = await _citiesService.Remove(id);
            if (city == null) return NotFound("Cidade nao encontrada!");
            TeamService.Update(city.Id, null);

            return NoContent();
        }
    }
}

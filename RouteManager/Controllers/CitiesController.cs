using Microsoft.AspNetCore.Mvc;
using RouteManager.Models;
using RouteManager.Services;
using System.Linq;
using System.Threading.Tasks;

namespace RouteManager.Controllers
{
    public class CitiesController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var cities = await CityService.Get();

            return View(cities);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));
            var city = await CityService.Get(id);

            return View(city);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,State")] CityViewModel city)
        {
            if (ModelState.IsValid)
            {
                var response = await CityService.Create(city);
                if (response.StatusCode != System.Net.HttpStatusCode.OK) return RedirectToAction(nameof(Index));
            }

            return View(city);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var teamsInCity = await TeamService.GetTeamsByCity(id);
            if (teamsInCity.Any()) return RedirectToAction(nameof(Index));

            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));
            var response = await CityService.Delete(id);
            if (response.StatusCode != System.Net.HttpStatusCode.OK) return RedirectToAction(nameof(Index));

            return View();
        }

    }
}
using Microsoft.AspNetCore.Mvc;
using RouteManager.Models;
using RouteManager.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouteManager.Controllers
{
    public class TeamsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var teams = await TeamService.Get();
            return View(teams);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));
            var team = await TeamService.Get(id);
            return View(team);
        }

        public async Task<IActionResult> Create()
        {
            var availablePeopleToTeam = await GetAvailablePeopleToTeam();
            ViewBag.AvailablePeopleToTeam = availablePeopleToTeam;

            var cities = await CityService.Get();
            ViewBag.Cities = cities;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name")] TeamViewModel team)
        {
            var newMembers = new List<PersonViewModel>();

            if (ModelState.IsValid)
            {
                var membersSelectedIds = Request.Form["checkedMembers"].ToList();

                var citiesSelectedId = Request.Form["selectedCity"].FirstOrDefault();
                if (string.IsNullOrEmpty(citiesSelectedId)) return RedirectToAction(nameof(Create));

                var city = await CityService.Get(citiesSelectedId);
                if (city == null) return RedirectToAction(nameof(Create));

                if (membersSelectedIds.Count == 0)
                {
                    ModelState.AddModelError(nameof(team), "Um time deve ter pelo menos um membro!");
                    return RedirectToAction(nameof(Create));
                }

                var teams = await TeamService.Get();
                if (teams.Any(x => x.Name == team.Name))
                {
                    ModelState.AddModelError(nameof(team), "Ja existe um time com esse nome!");
                    return RedirectToAction(nameof(Create));
                }

                foreach (var personId in membersSelectedIds)
                {
                    var person = await PersonService.Get(personId);
                    newMembers.Add(person);
                }

                team.Members = newMembers;
                team.City = city;
                await TeamService.Create(team);
                return RedirectToAction(nameof(Index));                
            }

            return View(team);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));

            var team = await TeamService.Get(id);

            var availablePeopleToTeam = await GetAvailablePeopleToTeam();
            ViewBag.AvailablePeopleToTeam = availablePeopleToTeam.ToList();

            var cities = await CityService.Get();
            ViewBag.Cities = cities;

            var teamMembers = new List<PersonViewModel>();
            team.Members.ForEach(person => teamMembers.Add(person));

            return View(team);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,City,IsAvailable")] TeamViewModel team)
        {
            var availablePeopleToTeam = await GetAvailablePeopleToTeam();
            ViewBag.AvailablePeopleToTeam = availablePeopleToTeam;

            var cities = await CityService.Get();
            ViewBag.Cities = cities;

            if (!team.Id.Equals(id)) return RedirectToAction(nameof(Index));

            var teamToUpdate = await TeamService.Get(id);
            var city = await CityService.Get(team.City.Id);
            team.City = city;

            var membersIdToAdd = Request.Form["checkedMembersToAdd"].ToList();
            if (membersIdToAdd.Count != 0)
            {
                membersIdToAdd.ForEach(async personId =>
                {
                    var person = await PersonService.Get(personId);
                    await TeamService.UpdateToAddMember(id, person);
                });
            }

            var membersIdToRemove = Request.Form["checkedMembersToRemove"].ToList();
            if (membersIdToRemove.Count != 0)
            {
                membersIdToRemove.ForEach(async personId =>
                {
                    var person = await PersonService.Get(personId);
                    await TeamService.UpdateToRemoveMember(id, person);
                });
            }

            var response = await TeamService.Update(id, team);
            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            return View(team);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));

            var response = await TeamService.Delete(id);
            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            return View();
        }

        static async Task<IEnumerable<PersonViewModel>> GetAvailablePeopleToTeam()
        {
            var members = await PersonService.Get();
            return members.Where(x => string.IsNullOrEmpty(x.CurrentTeam)).ToList();
        }

    }
}

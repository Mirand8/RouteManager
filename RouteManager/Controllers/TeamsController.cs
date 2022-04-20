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
            return View(await TeamService.Get());
        }


        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));
            return View(await TeamService.Get(id));
        }

        public async Task<IActionResult> Create()
        {
            var peopleAvailable = await GetAvailableMembers();
            ViewBag.PeopleAvailable = peopleAvailable;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TeamViewModel team)
        {
            var membersSelected = new List<PersonViewModel>();

            if (ModelState.IsValid)
            {
                var members = Request.Form["checkMembers"].ToList();
                if (members.Count == 0) return RedirectToAction(nameof(Create));
                members.ForEach(async personId => membersSelected.Add(await PersonService.Get(personId)));
                team.Members = membersSelected;

                await TeamService.Create(team);
                return RedirectToAction(nameof(Index));
            }

            return View(team);
        }


        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));

            var team = await TeamService.Get(id);
            var members = await PersonService.Get();
            var availableMembers = members.Where(member => member.IsAvailableToTeam);
            var teamMembers = new List<PersonViewModel>();

            team.Members.ForEach(person => teamMembers.Add(person));

            ViewBag.AvailableMembers = availableMembers;
            ViewBag.TeamMembers = teamMembers;

            return View(team);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(string id, TeamViewModel team)
        {
            if (!team.Id.Equals(id)) return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid) return View(team);

            var membersToAdd = Request.Form["checkAvailableMembersToAdd"].ToList();
            var membersToRemove = Request.Form["checkAvailableMembersToRemove"].ToList();
            var teamUpdate = await TeamService.Get(id);

            if (membersToAdd.Count != 0) membersToAdd.ForEach(async personId => await TeamService.UpdateInsert(id, await PersonService.Get(personId)));
            if (membersToRemove.Count != 0) membersToRemove.ForEach(async personId => await TeamService.UpdateRemove(id, await PersonService.Get(personId)));

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


        static async Task<IEnumerable<PersonViewModel>> GetAvailableMembers()
        {
            var members = await PersonService.Get();
            return members.Where(x => x.IsAvailableToTeam);
        }

    }
}

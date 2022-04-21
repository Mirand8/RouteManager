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
            var availableMembers = await GetAvailableMembers();
            if (!availableMembers.Any() || availableMembers == null) ViewBag.AvailableMembers = new List<PersonViewModel>();
            else ViewBag.AvailableMembers = availableMembers;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(TeamViewModel team)
        {
            var membersSelected = new List<PersonViewModel>();

            if (ModelState.IsValid)
            {
                var members = Request.Form["checkedMembers"].ToList();
                if (members.Count == 0)
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

                else
                {
                    members.ForEach(async id =>
                    {
                        var person = await PersonService.Get(id);
                        await PersonService.UpdateAvailablety(id, person);
                        membersSelected.Add(person);
                    });
                    team.Members = membersSelected;

                    await TeamService.Create(team);

                    return View(nameof(Create));
                }
            }
            return View(team);
        }


        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));

            var team = await TeamService.Get(id);
            var people = await PersonService.Get();
            var availablePeopleToTeam = people.Where(member => member.IsAvailableToTeam);

            var members = new List<PersonViewModel>();
            team.Members.ForEach(person => members.Add(person));

            ViewBag.AvailablePeopleToTeam = availablePeopleToTeam;
            ViewBag.Members = members;

            return View(team);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(string id, TeamViewModel team)
        {
            if (!team.Id.Equals(id)) return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid) return View(team);

            var membersToAdd = Request.Form["checkedMembersToAdd"].ToList();
            var membersToRemove = Request.Form["checkedMembersToRemove"].ToList();
            var teamUpdate = await TeamService.Get(id);

            if (membersToAdd.Count != 0) 
                membersToAdd.ForEach(async personId =>
                {
                    var person = await PersonService.Get(personId);
                    await PersonService.UpdateAvailablety(personId, person);
                    await TeamService.UpdateInsert(id, person);
                });
            if (membersToRemove.Count != 0) 
                membersToRemove.ForEach(async personId =>
                {
                    var person = await PersonService.Get(personId);
                    await PersonService.UpdateAvailablety(personId, person);
                    await TeamService.UpdateRemove(id, await PersonService.Get(personId));
                });

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
            return members.Where(x => x.IsAvailableToTeam).ToList();
        }

    }
}

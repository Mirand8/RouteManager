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
            var availablePeopleToTeam = await GetAvailablePeopleToTeam();
            ViewBag.AvailablePeopleToTeam = availablePeopleToTeam;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,IsAvailable")] TeamViewModel team)
        {
            var newMembers = new List<PersonViewModel>();

            if (ModelState.IsValid)
            {
                var membersSelectedIds = Request.Form["checkedMembers"].ToList();
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

                membersSelectedIds.ForEach(async personId =>
                {
                    var person = await PersonService.Get(personId);
                    newMembers.Add(person);
                });
                team.Members = newMembers;

                await TeamService.Create(team);
                return RedirectToAction(nameof(Create));                
            }

            return View(team);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));

            var team = await TeamService.Get(id);

            var availablePeopleToTeam = await GetAvailablePeopleToTeam();
            ViewBag.AvailablePeopleToTeam = availablePeopleToTeam.ToList();

            var teamMembers = new List<PersonViewModel>();
            team.Members.ForEach(person => teamMembers.Add(person));

            return View(team);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string teamId, TeamViewModel team)
        {
            if (!team.Id.Equals(teamId)) return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                var teamUpdate = await TeamService.Get(teamId);

                var membersIdToAdd = Request.Form["checkedMembersToAdd"].ToList();
                if (membersIdToAdd.Count != 0)
                {
                    membersIdToAdd.ForEach(async personId =>
                    {
                        var person = await PersonService.Get(personId);
                        await TeamService.UpdateToAddMember(teamId, person);
                    });
                }

                var membersIdToRemove = Request.Form["checkedMembersToRemove"].ToList();
                if (membersIdToRemove.Count != 0)
                {
                    membersIdToRemove.ForEach(async personId =>
                    {
                        var person = await PersonService.Get(personId);
                        await TeamService.UpdateToRemoveMember(teamId, person);
                    });
                }

                var response = await TeamService.Update(teamId, team);
                if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            }
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
            return members.Where(x => !x.IsOnTeam).ToList();
        }

    }
}

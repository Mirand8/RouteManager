using Microsoft.AspNetCore.Mvc;
using RouteManager.Models;
using RouteManager.Services;
using System.Threading.Tasks;

namespace RouteManager.Controllers
{
    public class PeopleController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var people = await PersonService.Get();

            return View(people);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));
            var person = await PersonService.Get(id);

            return View(person);
        }

        public async Task<IActionResult> Create(PersonViewModel person)
        {
            if (string.IsNullOrEmpty(person.Name)) return View(person);
            await PersonService.Create(person);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));
            var person = await PersonService.Get(id);

            return View(person);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, PersonViewModel person)
        {
            if (!id.Equals(person.Id)) return RedirectToAction(nameof(Index));

            var response = await PersonService.Update(id, person);
            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            return View(person);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Index));

            var response = await PersonService.Delete(id);
            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            return View(nameof(Index));
          }

    }
}

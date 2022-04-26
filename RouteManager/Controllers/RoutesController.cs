using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RouteManager.Models;
using RouteManager.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace RouteManager.Controllers
{
    public class RoutesController : Controller
    {
        readonly ExcelFileService _excelFilesService;

        public RoutesController(ExcelFileService excelFileService)
        {
            _excelFilesService = excelFileService;
        }

        public IActionResult Index()
        {
            var headers = new List<string>();

            var excelFile = _excelFilesService.Get();
            if (excelFile == null)
            {
                ViewData["excelFileEmpty"] = "Nenhum arquivo excel foi importado ainda!";
            }
            foreach (var item in excelFile.ExcelFiles.First()) headers.Add(item.Name);

            headers.RemoveAll(x => x == "CONTRATO" || x == "ASSINANTE" || x == "ENDEREÇO" || x == "CEP" || x == "OS" || x == "TIPO OS");

            ViewBag.FileName = excelFile.FileName;
            ViewBag.Headers = headers;
            ViewBag.Services = new SelectList(_excelFilesService.GetServices());
            ViewBag.Cities = new SelectList(_excelFilesService.GetCities());

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(Route route)
        {
            try
            {
                if (route.Service == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                var stream = new MemoryStream();
                var routeWordDocument = DocX.Create(stream);

                var paragraph = routeWordDocument.InsertParagraph();
                paragraph.Append($"ROTA TRABALHO - {DateTime.Now:d}").Font("Times New Roman").FontSize(18).Bold().Alignment = Xceed.Document.NET.Alignment.center;

                paragraph = routeWordDocument.InsertParagraph();
                paragraph.Append("RETORNOS").Font("Times New Roman").FontSize(15).Bold().Alignment = Xceed.Document.NET.Alignment.center;

                var teams = await TeamService.Get();
                var availableTeam = teams.Where(x => x.IsAvailable);

                if (route.City != null)
                    availableTeam = availableTeam.ToList().FindAll(x => x.City.Name.ToUpper() == route.City.ToUpper());

                if (!teams.Any())
                {
                    TempData["noTeamFound"] = "Nenhuma equipe foi encontrada!";
                    return RedirectToAction(nameof(Index));
                }

                var excel = _excelFilesService.Get();
                excel.ExcelFiles.RemoveAll(x => x.GetValue("SERVIÇO") != route.Service);

                var validTeams = new List<TeamViewModel>();
                foreach (var team in teams)
                {
                    var teamData = excel.ExcelFiles.Find(x => x.GetValue("CIDADE") == team.City.Name.ToUpper());
                    if (teamData != null) validTeams.Add(team);
                }

                if (!validTeams.Any()) return RedirectToAction(nameof(Index));

                foreach (var team in validTeams)
                {
                    var teamData = excel.ExcelFiles.Find(x => x.GetValue("CIDADE") == team.City.Name.ToUpper());

                    paragraph = routeWordDocument.InsertParagraph();
                    paragraph = routeWordDocument.InsertParagraph();
                    paragraph = routeWordDocument.InsertParagraph();
                    paragraph = routeWordDocument.InsertParagraph();
                    paragraph.Append($"Nome da Equipe: {team.Name}").Font("Times New Roman").FontSize(15).Bold();

                    paragraph = routeWordDocument.InsertParagraph();
                    paragraph = routeWordDocument.InsertParagraph();
                    teamData.TryGetValue("CONTRATO", out var contrato);
                    teamData.TryGetValue("ASSINANTE", out var assinante);
                    teamData.TryGetValue("PERÍODO", out var periodo);
                    paragraph.Append($"Contrato: {contrato}  -  Assinante: {assinante}  -  Período: {periodo}").Font("Times New Roman").FontSize(14).Bold().UnderlineStyle(Xceed.Document.NET.UnderlineStyle.singleLine);
                    paragraph = routeWordDocument.InsertParagraph();
                    paragraph.Append($"Endereço: {teamData.GetValue("ENDEREÇO")} - {teamData.GetValue("CEP")}").Font("Times New Roman").FontSize(14);

                    paragraph = routeWordDocument.InsertParagraph();
                    paragraph.Append($"O.S: {teamData.GetValue("OS")}  -  ").Font("Times New Roman").FontSize(14);
                    paragraph.Append($"TIPO O.S: {teamData.GetValue("TIPO OS")}").Font("Times New Roman").FontSize(14).Color(Color.White).Highlight(Xceed.Document.NET.Highlight.red);

                    foreach (var rout in route.Routes)
                    {
                        paragraph = routeWordDocument.InsertParagraph();
                        paragraph.Append($"{rout[0] + rout[1..].ToLower()}: {teamData.GetValue(rout)}").Font("Times New Roman").FontSize(14);
                    }
                }
                routeWordDocument.Save();

                return File(stream.ToArray(), "application/octet-stream", $"Rotas-{DateTime.Now}.docx");
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using OfficeOpenXml;
using RouteManager.Models;
using RouteManager.Services;

namespace RouteManager.Controllers
{
    public class FilesController : Controller
    {
        static IWebHostEnvironment _webHostEnvironment;

        public static List<List<string>> routes = new();
        public static List<string> headers = new();
        public static List<string> services = new();
        public static string serviceName;
        public static string cityId;
        public static string downloadFile;

        public FilesController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            List<TeamViewModel> teams = new();
            List<TeamViewModel> availableTeams = new();

            if (cityId != null)
                teams = await TeamService.GetTeamsByCity(cityId);
            
            if (teams != null)
                availableTeams = (List<TeamViewModel>) teams.Where(team => team.IsAvailable);

            ViewBag.AvailableTeams = availableTeams;
            ViewBag.Headers = headers;

            return View();
        }

        public IActionResult Upload()
        {
            return View();
        }

        public IActionResult UploadFile()
        {
            var files = HttpContext.Request.Form.Files;

            if (files.Count > 0)
            {
                var routesFromExcel = new List<List<string>>();
                var headersFromExcel = new List<string>();

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage(files[0].OpenReadStream());
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                var columnCount = worksheet.Dimension.End.Column;
                var rowCount = worksheet.Dimension.End.Row;

                int cepColumn = 0;
                int serviceColumn = 0;

                for (var column = 1; column < columnCount; column++)
                {
                    headersFromExcel.Add(worksheet.Cells[1, column].Value.ToString());

                    if (worksheet.Cells[1, column].Value.ToString().ToUpper().Equals("CEP"))
                        cepColumn = column - 1;

                    if (worksheet.Cells[1, column].Value.ToString().ToUpper().Equals("SERVIÇO") )
                        serviceColumn = column;
                }

                headers = headersFromExcel;

                worksheet.Cells[2, 1, rowCount, columnCount].Sort(cepColumn, false);

                var servicesRaw = new List<string>();
                for (var row = 1; row < rowCount; row++)
                {
                    var rowContent = new List<string>();

                    for (var column = 1; column < columnCount; column++)
                    {
                        servicesRaw.Add(worksheet.Cells[row, serviceColumn].Value.ToString().ToUpper());

                        var content = worksheet.Cells[row, column].Value?.ToString() ?? "";
                        rowContent.Add(content.ToString());
                    }
                    routesFromExcel.Add(rowContent);
                }

                services = servicesRaw.Distinct().ToList();
                services.RemoveAt(0);

                routes = routesFromExcel;

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Upload));
        }

    }
}

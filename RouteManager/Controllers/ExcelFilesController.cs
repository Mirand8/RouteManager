using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using RouteManager.Models;
using RouteManager.Services;

namespace RouteManager.Controllers
{
    public class ExcelFilesController : Controller
    {
        readonly ExcelFileService _excelService;

        public ExcelFilesController(ExcelFileService excelService)
        {
            _excelService = excelService;
        }

        public IActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index()
        {
            var files = HttpContext.Request.Form.Files;

            if ( files.Count != 0 || files.Any() )
            {
                if (!Path.GetExtension(files[0].FileName).Contains(".xlsx") || !Path.GetExtension(files[0].FileName).ToLowerInvariant().Contains(".xls"))
                {
                    TempData["invalidFile"] = "Formato de arquivo inválido!";
                    return RedirectToAction(nameof(UploadFile));
                }

                var xls = new XLWorkbook(files[0].OpenReadStream());
                var sheet = xls.Worksheets.First();

                var columns = new List<string>();
                var columnsCount = sheet.Columns().Count();

                var dataTable = new DataTable();

                for (int i = 1; i < columnsCount; i++)
                {
                    var col = sheet.Column(i).FirstCell().Value.ToString();
                    dataTable.Columns.Add(col);
                    columns.Add(col);
                }

                var line = 2;
                var @continue = true;

                while (@continue)
                {   
                    var data = new string[columns.Count];
                    int index = 0;
                    for (int i = 1; i <= columns.Count; i++)
                    {
                        data[index] = sheet.Column(i).Cell(line).Value.ToString();
                        index++;
                    }

                    @continue = data.Any(x => !string.IsNullOrEmpty(x));

                    if (@continue) dataTable.Rows.Add(data);

                    line++;
                }

                dataTable.DefaultView.Sort = "cep ASC";
                dataTable = dataTable.DefaultView.ToTable();

                var excel = new ExcelFile();

                for (int i = 1; i < dataTable.Rows.Count - 1; i++)
                {
                    var dictionary = new Dictionary<string, string>();
                    int j = 0;
                    foreach (var item in columns)
                    {
                        dictionary.Add(item, dataTable.Rows[i][j].ToString());
                        j++;
                    }
                    var json = JsonConvert.SerializeObject(dictionary);
                    var document = BsonSerializer.Deserialize<BsonDocument>(json);
                    excel.ExcelFiles.Add(document);
                }

                _excelService.Remove();
                _excelService.Create(excel);

                TempData["fileSucceded"] = "Arquivo salvo com sucesso!";

                return RedirectToAction(nameof(UploadFile));
            }

            return RedirectToAction(nameof(UploadFile));
        }

    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreCodeGenerator.Models;
using DotNetCodeGenerator.Domain.Entities;
using DotNetCodeGenerator.Domain.Entities.Enums;
using DotNetCodeGenerator.Domain.Helpers;
using DotNetCodeGenerator.Domain.Services;

namespace DotNetCoreCodeGenerator.Controllers
{
    public class HomeController : BaseController
    {
        public ITableService TableService { get; set; }

        public HomeController(ITableService _tableService)
        {
            TableService = _tableService;
        }
        public IActionResult Index()
        {
            var codeGeneratorResult = new CodeGeneratorResult();
            codeGeneratorResult.DatabaseMetadata = null;
            codeGeneratorResult.UserMessage = "Hi, dude, Generate the code of selected table \"Controller, Service, Repository and the SQL\" :)";
            codeGeneratorResult.UserMessageState = UserMessageState.Welcome;
            return View(codeGeneratorResult);
        }
        [HttpPost]
        public async Task<IActionResult> Index(CodeGeneratorResult codeGeneratorResult, string btnAction = "")
        {
            if (!String.IsNullOrEmpty(codeGeneratorResult.ConnectionString.ToStr().Trim())
                || !String.IsNullOrEmpty(codeGeneratorResult.MySqlConnectionString.ToStr().Trim()))
            {
                if (String.IsNullOrEmpty(codeGeneratorResult.SelectedTable.ToStr().Trim()))
                {
                    ModelState.AddModelError("SelectedTable", "Selected Table is required.");
                    return View(codeGeneratorResult);
                }
                else if (String.IsNullOrEmpty(codeGeneratorResult.ModifiedTableName.ToStr().Trim()))
                {
                    ModelState.AddModelError("ModifiedTableName", "Entity Name is required.");
                    return View(codeGeneratorResult);
                }
            }
            else if (String.IsNullOrEmpty(codeGeneratorResult.SqlCreateTableStatement.ToStr().Trim()))
            {
                ModelState.AddModelError("SqlCreateTableStatement", "Sql Create Table Statement is required.");
                return View(codeGeneratorResult);
            }

            if (btnAction.Equals("Generate Code", StringComparison.InvariantCultureIgnoreCase))
            {
                await TableService.GenerateCode(codeGeneratorResult);
            }
            else if (btnAction.Equals("Fill GridView", StringComparison.InvariantCultureIgnoreCase))
            {
                await TableService.FillGridView(codeGeneratorResult);
            }
            //Logger.Trace("XmlParserHelper.ToXml codeGeneratorResult " + XmlParserHelper.ToXml(codeGeneratorResult));
            return View(codeGeneratorResult);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

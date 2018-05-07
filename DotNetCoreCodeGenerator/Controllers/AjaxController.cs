using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCodeGenerator.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotNetCoreCodeGenerator.Controllers
{
    public class AjaxController : BaseController
    {
        private readonly ILogger _logger;
        private TableService TableService { get; set; }

        public AjaxController(TableService _tableService, ILogger<AjaxController> logger)
        {
            _logger = logger;
            TableService = _tableService;
        }
       // public async Task<IActionResult> GetTables(String connectionString = "", string mySqlConnectionString = "")


        public IActionResult GetTables(String connectionString = "", string mySqlConnectionString = "")
        {
            if (String.IsNullOrEmpty(connectionString) && String.IsNullOrEmpty(mySqlConnectionString))
            {
                //   return Json("", JsonRequestBehavior.AllowGet);
                return Ok("");
            }
            if (!String.IsNullOrEmpty(connectionString))
            {
                var allTablesMetaData = TableService.GetAllTablesFromCache(connectionString);
                var resultHtml = (from t in allTablesMetaData.Tables
                                  select new
                                  {
                                      TableNameWithSchema = t.TableNameWithSchema,
                                      DatabaseTableName = t.DatabaseTableName + "-" + t.SuggestedEntityName
                                  }).ToList();

                resultHtml.Insert(0, new { TableNameWithSchema = "Select a Table from SqlServer", DatabaseTableName = "" });
                //return Json(resultHtml, JsonRequestBehavior.AllowGet);
                return Ok(resultHtml);
            }
            else if (!String.IsNullOrEmpty(mySqlConnectionString))
            {
                var allTablesMetaData = TableService.GetAllMySqlTables(mySqlConnectionString);
                var resultHtml = (from t in allTablesMetaData.Tables
                                  select new
                                  {
                                      TableNameWithSchema = t.TableNameWithSchema,
                                      DatabaseTableName = t.DatabaseTableName + "-" + t.SuggestedEntityName
                                  }).ToList();

                resultHtml.Insert(0, new { TableNameWithSchema = "Select a Table From MySql", DatabaseTableName = "" });
                // return Json(resultHtml, JsonRequestBehavior.AllowGet);
                return Ok(resultHtml);
            }

           // return Json("", JsonRequestBehavior.AllowGet);
                   return Ok("");
        }
    }
}
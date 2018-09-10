using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCodeGenerator.Domain.Helpers;
using DotNetCodeGenerator.Domain.Services;
using DotNetCoreCodeGenerator.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HelpersProject;

namespace DotNetCoreCodeGenerator.Controllers
{
    public class AjaxController : BaseController
    {
        private readonly ILogger<AjaxController> _logger;
        private ITableService TableService { get; set; }

        public AjaxController(ITableService _tableService, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AjaxController>();
            TableService = _tableService;
        }
        // public async Task<IActionResult> GetTables(String connectionString = "", string mySqlConnectionString = "")


        [HttpPost]
        //  public IActionResult GetTables([FromBody]string connectionString = "", [FromBody]string mySqlConnectionString = "")
        public IActionResult GetTables([FromBody]AjaxConnectionString ajaxConnectionString = null)

        {
            string connectionString = ajaxConnectionString.ConnectionString.ToStr().Trim();
            string mySqlConnectionString = ajaxConnectionString.MySqlConnectionString.ToStr().Trim();
            if (String.IsNullOrEmpty(connectionString) && String.IsNullOrEmpty(mySqlConnectionString))
            {
                //   return Json("", JsonRequestBehavior.AllowGet);
                return Ok("");
            }
            string jsonData = "";
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
                // return Ok(resultHtml);
                 jsonData = JsonConvert.SerializeObject(resultHtml);
       
            }
            else if (!String.IsNullOrEmpty(mySqlConnectionString))
            {
                var allTablesMetaData = TableService.GetAllMySqlTablesFromCache(mySqlConnectionString);
                var resultHtml = (from t in allTablesMetaData.Tables
                                  select new
                                  {
                                      TableNameWithSchema = t.TableNameWithSchema,
                                      DatabaseTableName = t.DatabaseTableName + "-" + t.SuggestedEntityName
                                  }).ToList();

                resultHtml.Insert(0, new { TableNameWithSchema = "Select a Table From MySql", DatabaseTableName = "" });
                jsonData = JsonConvert.SerializeObject(resultHtml);
       
            }

            // return Json("", JsonRequestBehavior.AllowGet);
            return Content(jsonData, "application/json");
        }
    }
}
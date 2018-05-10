using DotNetCodeGenerator.Domain.Entities;
using DotNetCodeGenerator.Domain.Entities.Enums;
using DotNetCodeGenerator.Domain.Helpers;
using DotNetCodeGenerator.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetCodeGenerator.Domain.Services
{
    public class TableService : ITableService
    {
        public ISqlParserHelper _sqlParserHelper { get; set; }
        public ITableRepository _tableRepository { get; set; }
        public ICodeProducerHelper _codeProducerHelper { get; set; }

        public ILogger Logger;

        private IMemoryCache cache;



        public TableService(IMemoryCache cache,
            ITableRepository _tableRepository,
            ICodeProducerHelper _codeProducerHelper,
            ISqlParserHelper _sqlParserHelper,
            ILogger<TableService> logger
            )
        {
            this._tableRepository = _tableRepository;
            this._codeProducerHelper = _codeProducerHelper;
            this._sqlParserHelper = _sqlParserHelper;
            this.cache = cache;
            this.Logger = logger;
        }

        public DatabaseMetadata GetAllTablesFromCache(String connectionString)
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddMinutes(1);
            options.SlidingExpiration = TimeSpan.FromMinutes(1);
            options.Priority = CacheItemPriority.Normal;

            string key = connectionString;
            var items = cache.Get<DatabaseMetadata>(key);
            if (items == null)
            {
                items = GetAllTables(connectionString);
                cache.Set(key, items, options);
                Logger.LogInformation("Setting  Sql Server Database Metadata To CACHE");
            }
            else
            {
                Logger.LogInformation("Getting Sql Server Database Metadata To CACHE");
            }
            return items;
        }
        public DatabaseMetadata GetAllMySqlTablesFromCache(String connectionString)
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddMinutes(1);
            options.SlidingExpiration = TimeSpan.FromMinutes(1);
            options.Priority = CacheItemPriority.Normal;

            string key = connectionString;
            var items = cache.Get<DatabaseMetadata>(key);
            if (items == null)
            {
                items = GetAllMySqlTables(connectionString);
                cache.Set(key, items, options);
                Logger.LogInformation("Setting MySql Database Metadata To CACHE");
            }
            else
            {
                Logger.LogInformation("Getting MySql Database Metadata To CACHE");
            }
            return items;
        }
        public DatabaseMetadata GetAllMySqlTables(String connectionString)
        {
            return _tableRepository.GetAllMySqlTables(connectionString);
        }

        public DatabaseMetadata GetAllTables(String connectionString)
        {
            return _tableRepository.GetAllTables(connectionString);
        }
        public DataSet GetDataSet(string sqlCommand, string connectionString)
        {
            return _tableRepository.GetDataSet(sqlCommand, connectionString);
        }
        public async Task FillGridView(CodeGeneratorResult codeGeneratorResult)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var databaseMetaData = new DatabaseMetadata();
                if (!String.IsNullOrEmpty(codeGeneratorResult.ConnectionString))
                {
                    databaseMetaData = this.GetAllTablesFromCache(codeGeneratorResult.ConnectionString);
                    _tableRepository.GetSelectedTableMetaData(databaseMetaData, codeGeneratorResult.SelectedTable);
                }
                else if (!String.IsNullOrEmpty(codeGeneratorResult.MySqlConnectionString))
                {
                    databaseMetaData = this.GetAllMySqlTablesFromCache(codeGeneratorResult.MySqlConnectionString);
                    _tableRepository.GetSelectedMysqlTableMetaData(databaseMetaData, codeGeneratorResult.SelectedTable);
                }
                codeGeneratorResult.DatabaseMetadata = databaseMetaData;
            });
            codeGeneratorResult.UserMessage = codeGeneratorResult.SelectedTable + " table metadata is populated to GridView. You are so close, Do not give up until you make it, dude :)";
            codeGeneratorResult.UserMessageState = UserMessageState.Success;
            await task;

        }
        public async Task GenerateCode(CodeGeneratorResult codeGeneratorResult)
        {
            DatabaseMetadata databaseMetaData = new DatabaseMetadata();

            databaseMetaData = await GetDatabaseMetaDataAsync(codeGeneratorResult, databaseMetaData);

            _codeProducerHelper.CodeGeneratorResult = codeGeneratorResult;
            _codeProducerHelper.DatabaseMetadata = databaseMetaData;


            var tasks = new List<Task>();
            // Database related code.
            if (databaseMetaData.DatabaseType == DatabaseType.MsSql || databaseMetaData.DatabaseType == DatabaseType.UnKnown)
            {
                tasks.Add(Task.Factory.StartNew(() => { _codeProducerHelper.GenerateSaveOrUpdateStoredProcedure(); }));
                tasks.Add(Task.Factory.StartNew(() => { _codeProducerHelper.GenerateSqlRepository(); }));
                tasks.Add(Task.Factory.StartNew(() => { _codeProducerHelper.GenerateStoredProcExecutionCode(); }));
            }
            else if (databaseMetaData.DatabaseType == DatabaseType.MySql)
            {
                tasks.Add(Task.Factory.StartNew(() => { _codeProducerHelper.GenerateMySqlSaveOrUpdateStoredProcedure(); }));
                tasks.Add(Task.Factory.StartNew(() => { _codeProducerHelper.GenereateMySqlRepository(); }));
            }

            // c# code for both database.
            tasks.Add(Task.Factory.StartNew(() => { _codeProducerHelper.GenerateWebApiController(); }));
            tasks.Add(Task.Factory.StartNew(() => { _codeProducerHelper.GenerateTableServices(); }));
            tasks.Add(Task.Factory.StartNew(() => { _codeProducerHelper.GenerateTableItem(); }));
            tasks.Add(Task.Factory.StartNew(() => { _codeProducerHelper.GenerateNewInstance(); }));
            tasks.Add(Task.Factory.StartNew(() => { _codeProducerHelper.GenerateAspMvcControllerClass(); }));




            await Task.WhenAll(tasks);

            var tableName = codeGeneratorResult.ModifiedTableName.ToStr(codeGeneratorResult.SelectedTable);
            codeGeneratorResult = _codeProducerHelper.CodeGeneratorResult;
            codeGeneratorResult.DatabaseMetadata = databaseMetaData;
            codeGeneratorResult.UserMessage = tableName + " table codes are created. You made it dude, Congratulation :)";
            codeGeneratorResult.UserMessageState = UserMessageState.Success;
        }
        private async Task<DatabaseMetadata> GetDatabaseMetaDataAsync(CodeGeneratorResult codeGeneratorResult, DatabaseMetadata databaseMetaData)
        {
            var t = Task<DatabaseMetadata>.Factory.StartNew(() =>
           {
               if (!String.IsNullOrEmpty(codeGeneratorResult.ConnectionString))
               {
                   databaseMetaData = this.GetAllTablesFromCache(codeGeneratorResult.ConnectionString);
                   _tableRepository.GetSelectedTableMetaData(databaseMetaData, codeGeneratorResult.SelectedTable);
               }
               else if (!String.IsNullOrEmpty(codeGeneratorResult.MySqlConnectionString))
               {
                   databaseMetaData = this.GetAllMySqlTablesFromCache(codeGeneratorResult.MySqlConnectionString);
                   _tableRepository.GetSelectedMysqlTableMetaData(databaseMetaData, codeGeneratorResult.SelectedTable);
               }
               else if (!String.IsNullOrEmpty(codeGeneratorResult.SqlCreateTableStatement))
               {
                   databaseMetaData = _sqlParserHelper.ParseSqlCreateStatement(codeGeneratorResult.SqlCreateTableStatement);
               }

               return databaseMetaData;

           });
            await t;
            return t.Result;
        }
    }
}

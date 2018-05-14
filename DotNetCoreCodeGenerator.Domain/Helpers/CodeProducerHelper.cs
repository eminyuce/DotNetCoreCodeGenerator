using DotNetCodeGenerator.Domain.Repositories;
using DotNetCodeGenerator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotNetCodeGenerator.Domain.Services;
using Microsoft.Extensions.Logging;

namespace DotNetCodeGenerator.Domain.Helpers
{
    public class CodeProducerHelper : ICodeProducerHelper
    {
        public TableService TableService { get; set; }

        public const string DEFAULT_NAMESPACE = "Test";


        private readonly ILogger _logger;

        public CodeProducerHelper(ILogger<CodeProducerHelper> logger)
        {
            _logger = logger;
        }


        public CodeGeneratorResult CodeGeneratorResult { get; set; }
        public DatabaseMetadata DatabaseMetadata { get; set; }

        private void GetMySqlDatabaseUtilityParameters(List<TableRowMetaData> kontrolList, StringBuilder method, String commandText = "", bool isSp = false)
        {
            String realEntityName = CodeGeneratorResult.SelectedTable;
            method.AppendLine(String.Format(" String commandText = @\"CALL {0}\";", commandText));
            method.AppendLine(" var parameterList = new List<MySqlParameter>();");


            foreach (TableRowMetaData item in kontrolList)
            {
                var sqlParameter = GeneralHelper.GetUrlString(item.ColumnName);
                if (item.DataType.IndexOf("xml") > -1 || item.DataType.IndexOf("varchar") > -1 || item.DataType.IndexOf("text") > -1 || item.DataType.IndexOf("nchar") > -1)
                {
                    method.AppendLine("parameterList.Add(new MySqlParameter(\"@" + item.ColumnNameInput + "\", item." +
                                      item.ColumnName + ".ToStr()));");
                }
                else
                {
                    method.AppendLine(" parameterList.Add(new MySqlParameter(\"@" + item.ColumnNameInput + "\", item." + item.ColumnName + "));");
                }

            }
        }


        public void GenereateMySqlRepository()
        {
            List<TableRowMetaData> kontrolList = DatabaseMetadata.SelectedTable.TableRowMetaDataList;
            StringBuilder method = new StringBuilder();


            try
            {

                String realEntityName = CodeGeneratorResult.SelectedTable;
                String modelName = CodeGeneratorResult.ModifiedTableName;
                String modifiedTableName = CodeGeneratorResult.ModifiedTableName;
                string selectedTable = DatabaseMetadata.SelectedTable.TableNameWithSchema;
                String entityPrefix = GeneralHelper.GetEntityPrefixName(realEntityName);
                String primaryKey = TableRowMetaDataHelper.GetPrimaryKeys(kontrolList);
                string primaryKeyOrginal = primaryKey;
                String staticText = CodeGeneratorResult.IsMethodStatic ? "static" : "";
                method.AppendLine(" public string ConnectionString = ConfigurationManager.ConnectionStrings[\"ConnectionStringKey\"].ConnectionString;");
                method.AppendLine("");
                method.AppendLine("");
                method.AppendLine("");
                method.AppendLine("public " + staticText + " List<" + modelName + "> Get" + modelName + "s()");
                method.AppendLine(" {");
                method.AppendLine(" var list = new List<" + modelName + ">();");
                String commandText = "SELECT * FROM " + selectedTable + " ORDER BY " + primaryKey + " DESC";
                method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
                method.AppendLine(" var parameterList = new List<MySqlParameter>();");

                GetMySqlDataSetCodeText(method);


                foreach (var ki in kontrolList)
                {
                    if (ki.ForeignKey)
                    {
                        String dataType = TableRowMetaDataHelper.GetSqlDataTypeFromColumnDataType(ki);
                        String cSharpType = TableRowMetaDataHelper.GetCSharpDataType(ki);
                        method.AppendLine("//" + ki.ColumnName);
                        method.AppendLine("public " + staticText + "  List<" + modelName + "> Get" + modelName + "By" + ki.ColumnName + "(" + cSharpType + " " + GeneralHelper.FirstCharacterToLower(ki.ColumnName) + ")");
                        method.AppendLine(" {");
                        method.AppendLine(" var list = new List<" + modelName + ">();");
                        commandText = "SELECT * FROM " + selectedTable + " WHERE " + ki.ColumnName + "=@" + ki.ColumnName + " ORDER BY " + primaryKey + " DESC";
                        method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
                        method.AppendLine(" var parameterList = new List<MySqlParameter>();");
                        method.AppendLine(" parameterList.Add(new MySqlParameter(\"@" + ki.ColumnName + "\", " + GeneralHelper.FirstCharacterToLower(ki.ColumnName) + "));");
                        GetMySqlDataSetCodeText(method);
                        method.AppendLine("");
                    }
                }
                method.AppendLine("public " + staticText + " void Delete" + modelName + "(int " + GeneralHelper.FirstCharacterToLower(primaryKey) + ")");
                method.AppendLine(" {");
                commandText = "DELETE FROM " + selectedTable + " WHERE " + primaryKey + "=@" + primaryKey;
                method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
                method.AppendLine(" var parameterList = new List<MySqlParameter>();");
                method.AppendLine(" parameterList.Add(new MySqlParameter(\"@" + primaryKey + "\", " + GeneralHelper.FirstCharacterToLower(primaryKey) + "));");
                method.AppendLine(" MySqlHelper.ExecuteNonQuery(ConnectionString, commandText, parameterList.ToArray());");
                method.AppendLine(" }");
                method.AppendLine("");
                method.AppendLine(" public " + staticText + " " + modelName + " Get" + modelName + "(int " + primaryKey + ")");
                method.AppendLine(" {");
                commandText = "SELECT * FROM " + selectedTable + " WHERE " + primaryKey + "=@" + primaryKey;
                method.AppendLine(" var resultEntity = new " + modifiedTableName + "();");
                method.AppendLine(String.Format("String commandText = @\"{0}\";", commandText));
                method.AppendLine(" var parameterList = new List<MySqlParameter>();");
                method.AppendLine(" parameterList.Add(new MySqlParameter(\"@" + primaryKey + "\", " + primaryKey + "));");
                method.AppendLine(" DataSet dataSet = MySqlHelper.ExecuteDataset(ConnectionString, commandText, parameterList.ToArray());");
                method.AppendLine(" resultEntity = dataSet.Tables[0].ToList<" + modifiedTableName + ">().FirstOrDefault();");
                method.AppendLine(" return resultEntity;");
                method.AppendLine(" }");

                entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");
                method.AppendLine("public " + staticText + " int SaveOrUpdate" + modelName + "( " + modelName + " item)");
                method.AppendLine(" {");

                string spName = "SaveOrUpdate" + modifiedTableName;
                spName = String.Format("{1}({0})",
                    String.Join(",",
                    kontrolList.Select(t =>
                    String.Format("@{0}", t.ColumnNameInput))), spName);
                GetMySqlDatabaseUtilityParameters(DatabaseMetadata.SelectedTable.TableRowMetaDataList, method, spName, true);
                method.AppendLine(" int id = MySqlHelper.ExecuteScalar(ConnectionString, commandText, parameterList.ToArray()).ToInt();");
                method.AppendLine(" return id;");
                method.AppendLine(" }");
                CodeGeneratorResult.MySqlDatabaseOperation = method.ToString();
                _logger.LogTrace("CodeGeneratorResult.MySqlDatabaseOperation:" 
                    + method.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                CodeGeneratorResult.MySqlDatabaseOperation = ex.Message;
            }

        }


        public void GenerateMySqlSaveOrUpdateStoredProcedure()
        {

            StringBuilder built = new StringBuilder();

            List<TableRowMetaData> list = DatabaseMetadata.SelectedTable.TableRowMetaDataList;
            TableRowMetaData prKey = TableRowMetaDataHelper.GetPrimaryKeysObj(list);
            try
            {
                String realEntityName = CodeGeneratorResult.SelectedTable;
                String modelName = CodeGeneratorResult.ModifiedTableName;
                String modifiedTableName = CodeGeneratorResult.ModifiedTableName;
                string selectedTable = DatabaseMetadata.SelectedTable.TableNameWithSchema;
                String entityPrefix = GeneralHelper.GetEntityPrefixName(realEntityName);
                String primaryKey = TableRowMetaDataHelper.GetPrimaryKeys(list);
                string primaryKeyOrginal = primaryKey;
                String staticText = CodeGeneratorResult.IsMethodStatic ? "static" : "";

                entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");



                built = new StringBuilder();
                built.AppendLine("CREATE PROCEDURE SaveOrUpdate" + modifiedTableName + "(");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    built.AppendLine("IN " + item.ColumnNameInput + " " + item.DataTypeMaxChar + comma);
                }

                built.Append(")");
                built.AppendLine("");
                built.AppendLine("BEGIN");


                built.AppendLine(" DECLARE MyId int;");
                built.AppendLine(" DECLARE CheckExists int;");

                built.AppendLine("  DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING");
                built.AppendLine("  BEGIN");
                built.AppendLine("  ROLLBACK;");
                built.AppendLine("  RESIGNAL;");
                built.AppendLine("  END;");

                built.AppendLine("");
                built.AppendLine("START TRANSACTION;");
                built.AppendLine("SET CheckExists = 0;");
                built.AppendLine("SET MyId = " + prKey.ColumnNameInput + ";");
                // SELECT count(*) INTO CheckExists from db_kodyazan.Test WHERE Id = MyId;
                built.AppendLine("SELECT COUNT(*) INTO CheckExists FROM " + selectedTable + " WHERE Id = MyId;");
                built.AppendLine("IF(CheckExists = 0) THEN ");
                built.AppendLine("  SET SQL_MODE = '';");
                built.AppendLine("INSERT INTO " + selectedTable + "(");

                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];

                    if (!item.PrimaryKey)
                        built.AppendLine(String.Format("`{0}`{1}", item.ColumnName, (i != (list.Count - 1) ? "," : "")));
                }

                built.AppendLine(") VALUES (");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    if (!item.PrimaryKey)
                        built.AppendLine("COALESCE(" + item.ColumnNameInput + "," + item.ColumnDefaultValue + ")" + comma);
                }

                built.AppendLine(");");
                built.AppendLine("");
                built.AppendLine(" SET MyId = LAST_INSERT_ID();");
                built.AppendLine("ELSE");
                built.AppendLine("UPDATE " + selectedTable + " SET");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    if (!item.PrimaryKey)
                    {
                        built.AppendLine(String.Format("`{0}`", item.ColumnName) + " = COALESCE(" + item.ColumnNameInput + "," + item.ColumnDefaultValue + ")" + comma);
                    }
                }

                built.AppendLine("WHERE " + String.Format("`{0}`", prKey.ColumnName) + "=MyId;");

                built.AppendLine(" END IF;");
                built.AppendLine("COMMIT;");
                built.AppendLine(" SELECT MyId;");
                built.AppendLine("END");
                CodeGeneratorResult.MySqlSaveOrUpdateStoredProc = built.ToString();
                _logger.LogTrace("CodeGeneratorResult.MySqlSaveOrUpdateStoredProc:" + built.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                CodeGeneratorResult.MySqlSaveOrUpdateStoredProc = ex.Message;
            }
        }
        public void GenerateSaveOrUpdateStoredProcedure()
        {

            StringBuilder built = new StringBuilder();

            List<TableRowMetaData> list = DatabaseMetadata.SelectedTable.TableRowMetaDataList;
            TableRowMetaData prKey = TableRowMetaDataHelper.GetPrimaryKeysObj(list);
            try
            {
                String realEntityName = CodeGeneratorResult.SelectedTable;
                String modelName = CodeGeneratorResult.ModifiedTableName;
                String modifiedTableName = CodeGeneratorResult.ModifiedTableName;
                string selectedTable = DatabaseMetadata.SelectedTable.TableNameWithSchema;
                String entityPrefix = GeneralHelper.GetEntityPrefixName(realEntityName);
                String primaryKey = TableRowMetaDataHelper.GetPrimaryKeys(list);
                string primaryKeyOrginal = primaryKey;
                String staticText = CodeGeneratorResult.IsMethodStatic ? "static" : "";

                entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");



                built = new StringBuilder();
                built.AppendLine("CREATE PROCEDURE  " + entityPrefix + "SaveOrUpdate" + modifiedTableName + "(");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    built.AppendLine("@" + GeneralHelper.GetUrlString(item.ColumnName) + " " + item.DataTypeMaxChar + " = " + (String.IsNullOrEmpty(item.ColumnDefaultValue) ? "NULL" : item.ColumnDefaultValue) + comma);
                }

                built.Append(")");
                built.AppendLine("AS");
                built.AppendLine("BEGIN");
                built.AppendLine("IF NOT EXISTS(SELECT  " + prKey.ColumnName + " FROM " + selectedTable + " WHERE " + prKey.ColumnName + "=@" + prKey.ColumnName + ") ");
                built.AppendLine("BEGIN");
                built.AppendLine("INSERT INTO " + selectedTable + "(");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    if (!item.PrimaryKey)
                        built.AppendLine(String.Format("[{0}]{1}", item.ColumnName, comma));
                }

                built.AppendLine(") VALUES (");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    if (!item.PrimaryKey)
                        built.AppendLine("@" + GeneralHelper.GetUrlString(item.ColumnName) + comma);
                }

                built.AppendLine(")");
                built.AppendLine("");
                built.AppendLine("SET @" + prKey.ColumnName + "=SCOPE_IDENTITY()");
                built.AppendLine("END");
                built.AppendLine("ELSE");
                built.AppendLine("BEGIN");
                built.AppendLine("UPDATE " + selectedTable + " SET");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    if (!item.PrimaryKey)
                    {
                        built.AppendLine(String.Format("[{0}]", item.ColumnName) + " = @" + GeneralHelper.GetUrlString(item.ColumnName) + comma);
                    }
                }
                built.AppendLine("");
                built.AppendLine("WHERE " + String.Format("[{0}]", prKey.ColumnName) + "=@" + prKey.ColumnName + ";");
                built.AppendLine("END");
                built.AppendLine("SELECT @" + prKey.ColumnName + " as " + prKey.ColumnName + "");
                built.AppendLine("END");

                CodeGeneratorResult.SqlSaveOrUpdateStoredProc = built.ToString();
                _logger.LogTrace("CodeGeneratorResult.SqlSaveOrUpdateStoredProc:" + built.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                CodeGeneratorResult.SqlSaveOrUpdateStoredProc = ex.Message;
            }
        }
        public void GenerateWebApiController()
        {
            List<TableRowMetaData> kontrolList = DatabaseMetadata.SelectedTable.TableRowMetaDataList;
            StringBuilder method = new StringBuilder();
            String realEntityName = CodeGeneratorResult.SelectedTable;
            String modelName = CodeGeneratorResult.ModifiedTableName;
            String modifiedTableName = CodeGeneratorResult.ModifiedTableName.ToStr().Trim();
            String nameSpace = CodeGeneratorResult.NameSpace.ToStr(DEFAULT_NAMESPACE);
            string selectedTable = DatabaseMetadata.SelectedTable.TableNameWithSchema;
            String entityPrefix = GeneralHelper.GetEntityPrefixName(realEntityName);
            String primaryKey = TableRowMetaDataHelper.GetPrimaryKeys(kontrolList);
            string primaryKeyOrginal = primaryKey;
            String staticText = CodeGeneratorResult.IsMethodStatic ? "static" : "";

            try
            {

                method.AppendLine("using System;");
                method.AppendLine("using System.Collections.Generic;");
                method.AppendLine("using System.Linq;");
                method.AppendLine("using System.Web.Http;");
                method.AppendLine("using System.Web.Http.Description;");
                method.AppendLine(String.Format("using {0}.Models;", nameSpace));
                method.AppendLine(String.Format("using {0}.Services;", nameSpace));

                method.AppendLine("namespace " + nameSpace + ".Controllers");
                method.AppendLine("{");

                //Documentation
                method.AppendLine("\t///<summary>");
                method.AppendLine("\t/// Controller for the " + modifiedTableName + "");
                method.AppendLine("\t///</summary>");

                method.AppendLine("\t[RoutePrefix(\"api/" + modifiedTableName + "\")]");
                method.AppendLine("\tpublic class " + modifiedTableName + "Controller : BaseController");
                method.AppendLine("\t{");

                method.AppendLine("\t\tprotected " + modifiedTableName + "Service GetService()");
                method.AppendLine("\t\t{");
                method.AppendLine("\t\t\treturn new " + modifiedTableName + "Service(GetRequestUserHostAddress(), GetRequestUserHostName());");
                method.AppendLine("\t\t}\r\n");

                ////List by Guids
                //foreach (Property p in m.Properties)
                //{
                //    if (p.Type == "Guid" && p.Name != "id")
                //    {
                //        //Documentation
                //        method.AppendLine("\t\t///<summary>");
                //        method.AppendLine("\t\t/// LIST all " +modifiedTableName + "s connected with the specifics " + UppercaseFirst(p.Name) + "");
                //        method.AppendLine("\t\t///</summary>");
                //        method.AppendLine("\t\t///<param name=\"ids\">List of " + UppercaseFirst(p.Name) + " Ids</param>");
                //        method.AppendLine("\t\t///<param name=\"all\">Include the inactive or not</param>");
                //        method.AppendLine("\t\t///<returns>200 - List of " +modifiedTableName + "</returns>");

                //        method.AppendLine("\t\t[HttpPost, Route(\"Get" +modifiedTableName + "By" + UppercaseFirst(p.Name) + "\"),  ResponseType(typeof(IEnumerable<" +modifiedTableName + ">))]");
                //        method.AppendLine("\t\tpublic IHttpActionResult Get" +modifiedTableName + "By" + UppercaseFirst(p.Name) + "([FromBody]IEnumerable<Guid> ids, [FromUri]Boolean all = false)");
                //        method.AppendLine("\t\t{");

                //        method.AppendLine("\t\t\ttry { return Ok(GetService().GetByIds<" +modifiedTableName + ">(ids, all, \"" + UppercaseFirst(p.Name) + "\")); }");
                //        method.AppendLine("\t\t\tcatch (Exception ex) { return base.ThreatExceptions(ex); }");

                //        method.AppendLine("\t\t}\r\n");
                //    }
                //}


                //Documentation
                method.AppendLine("\t\t///<summary>");
                method.AppendLine("\t\t///GET a specific " + modifiedTableName + "");
                method.AppendLine("\t\t///</summary>");
                method.AppendLine("\t\t///<param name=\"id\">" + modifiedTableName + " Id</param>");
                method.AppendLine("\t\t///<returns>200 - List of " + modifiedTableName + "</returns>");

                //Get
                method.AppendLine("\t\t[HttpGet,  ResponseType(typeof(" + modifiedTableName + "))]");
                method.AppendLine("\t\tpublic IHttpActionResult Get([FromUri]Guid id)");
                method.AppendLine("\t\t{");

                method.AppendLine("\t\t\ttry { return Ok(GetService().Get<" + modifiedTableName + ">(id)); }");
                method.AppendLine("\t\t\tcatch (Exception ex) { return base.ThreatExceptions(ex); }");

                method.AppendLine("\t\t}\r\n");


                //Documentation
                method.AppendLine("\t\t///<summary>");
                method.AppendLine("\t\t///GET all " + modifiedTableName + "s on Database");
                method.AppendLine("\t\t///</summary>");
                method.AppendLine("\t\t///<param name=\"all\">Include the inactive or not</param>");
                method.AppendLine("\t\t///<returns>200 - List of " + modifiedTableName + "</returns>");

                //List
                method.AppendLine("\t\t[HttpGet,  ResponseType(typeof(IEnumerable<" + modifiedTableName + ">))]");
                method.AppendLine("\t\tpublic IHttpActionResult Get([FromUri]Boolean all = false)");
                method.AppendLine("\t\t{");

                method.AppendLine("\t\t\ttry { return Ok(GetService().Get<" + modifiedTableName + ">(all)); }");
                method.AppendLine("\t\t\tcatch (Exception ex) { return base.ThreatExceptions(ex); }");

                method.AppendLine("\t\t}\r\n");


                //Documentation
                method.AppendLine("\t\t///<summary>");
                method.AppendLine("\t\t///POST a " + modifiedTableName + "");
                method.AppendLine("\t\t///</summary>");
                method.AppendLine("\t\t///<param name=\"value\">" + modifiedTableName + " to Post</param>");
                method.AppendLine("\t\t///<returns>200 - " + modifiedTableName + "</returns>");

                //Post
                method.AppendLine("\t\tpublic IHttpActionResult Post([FromBody]" + modifiedTableName + " value)");
                method.AppendLine("\t\t{");

                method.AppendLine("\t\t\tif (!ModelState.IsValid)");
                method.AppendLine("\t\t\t\treturn BadRequest(ModelState);\r\n");

                method.AppendLine("\t\t\ttry { return Created(\"Database\", GetService().Post(value)); }");
                method.AppendLine("\t\t\tcatch (Exception ex) { return base.ThreatExceptions(ex); }");

                method.AppendLine("\t\t}\r\n");


                //Documentation
                method.AppendLine("\t\t///<summary>");
                method.AppendLine("\t\t///PUT a " + modifiedTableName + "");
                method.AppendLine("\t\t///</summary>");
                method.AppendLine("\t\t///<param name=\"value\">" + modifiedTableName + " to Update</param>");
                method.AppendLine("\t\t///<returns>200 - " + modifiedTableName + "</returns>");


                //Put
                method.AppendLine("\t\tpublic IHttpActionResult Put([FromBody]" + modifiedTableName + " value)");
                method.AppendLine("\t\t{");

                method.AppendLine("\t\t\tif (!ModelState.IsValid)");
                method.AppendLine("\t\t\t\treturn BadRequest(ModelState);\r\n");

                //method.AppendLine("\t\t\ttry { return Ok(GetService().Put<" +modifiedTableName + ">(value, new List<String>() { \"" + m.Properties[1].Name + "\", \"" + m.Properties[2].Name + "\" })); }");
                method.AppendLine("\t\t\tcatch (Exception ex) { return base.ThreatExceptions(ex); }");

                method.AppendLine("\t\t}\r\n");



                //Documentation
                method.AppendLine("\t\t///<summary>");
                method.AppendLine("\t\t///Delete a " + modifiedTableName + ". A " + modifiedTableName + " is always soft deleted");
                method.AppendLine("\t\t///</summary>");
                method.AppendLine("\t\t///<param name=\"id\">" + modifiedTableName + " to Delete</param>");
                method.AppendLine("\t\t///<returns>200</returns>");

                //Delete
                method.AppendLine("\t\tpublic IHttpActionResult Delete([FromUri]Guid id)");
                method.AppendLine("\t\t{");

                method.AppendLine("\t\t\ttry { return Ok(GetService().Delete<" + modifiedTableName + ">(id)); }");
                method.AppendLine("\t\t\tcatch (Exception ex) { return base.ThreatExceptions(ex); }");

                method.AppendLine("\t\t}");


                method.AppendLine("\t\t}");
                method.AppendLine("}");
                CodeGeneratorResult.WebApiController = method.ToStr();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                CodeGeneratorResult.WebApiController = ex.Message;
            }

        }
        public void GenerateSqlRepository()
        {

            List<TableRowMetaData> kontrolList = DatabaseMetadata.SelectedTable.TableRowMetaDataList;
            StringBuilder method = new StringBuilder();


            try
            {

                String realEntityName = CodeGeneratorResult.SelectedTable;
                String modelName = CodeGeneratorResult.ModifiedTableName;
                String modifiedTableName = CodeGeneratorResult.ModifiedTableName;
                string selectedTable = DatabaseMetadata.SelectedTable.TableNameWithSchema;
                String entityPrefix = GeneralHelper.GetEntityPrefixName(realEntityName);
                String primaryKey = TableRowMetaDataHelper.GetPrimaryKeys(kontrolList);
                string primaryKeyOrginal = primaryKey;
                String staticText = CodeGeneratorResult.IsMethodStatic ? "static" : "";
                String nameSpace = CodeGeneratorResult.NameSpace.ToStr(DEFAULT_NAMESPACE);

                String repositoryName = CodeGeneratorResult.IsMethodStatic ?
          String.Format("{0}Repository", modelName) :
          String.Format("_{0}Repository", GeneralHelper.FirstCharacterToLower(modelName));

                method.AppendLine(String.Format("using {0}.DB;", nameSpace));
                method.AppendLine(String.Format("using {0}.Entities;", nameSpace));
                method.AppendLine("using System;");
                method.AppendLine("using System.Linq;");
                method.AppendLine("using System.Runtime.Caching;");
                method.AppendLine("using System.Text;");
                method.AppendLine("using System.NLog;");
                method.AppendLine("using System.Threading.Tasks;");
                method.AppendLine("");
                method.AppendLine(String.Format("namespace {0}.Repositories", nameSpace));
                method.AppendLine("{");
                method.AppendLine(String.Format("public class {0}Repository", modelName));
                method.AppendLine("{");
                method.AppendLine("private static readonly Logger Logger = LogManager.GetCurrentClassLogger();");
                method.AppendLine("private static string CacheKeyAllItems = \"" + modelName + "Cache\";");
                method.AppendLine("");
                if (!CodeGeneratorResult.IsMethodStatic)
                {
                    method.AppendLine(String.Format("public {0}Repository()", modelName.Replace("Nwm", "")));
                    method.AppendLine("{");
                    method.AppendLine("}");
                }

                method.AppendLine("");
                method.AppendLine("public string ConnectionString = ConfigurationManager.ConnectionStrings[\"ConnectionStringKey\"].ConnectionString;");
                method.AppendLine("public " + staticText + " List<" + modelName + "> Get" + modelName + "s()");
                method.AppendLine(" {");
                method.AppendLine(" var list = new List<" + modelName + ">();");
                String commandText = "SELECT * FROM " + selectedTable + " ORDER BY " + primaryKey + " DESC";
                // GetDatabaseUtilityParameters(kontrolList, method, commandText, false);
                method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
                method.AppendLine(" var parameterList = new List<SqlParameter>();");
                method.AppendLine(" var commandType = CommandType.Text;");
                GetDataSetCodeText(method);


                foreach (var ki in kontrolList)
                {
                    if (ki.ForeignKey)
                    {
                        String dataType = TableRowMetaDataHelper.GetSqlDataTypeFromColumnDataType(ki);
                        String cSharpType = TableRowMetaDataHelper.GetCSharpDataType(ki);
                        method.AppendLine("//" + ki.ColumnName);
                        method.AppendLine("public " + staticText + "  List<" + modelName + "> Get" + modelName + "By" + ki.ColumnName + "(" + cSharpType + " " + GeneralHelper.FirstCharacterToLower(ki.ColumnName) + ")");
                        method.AppendLine(" {");
                        method.AppendLine(" var list = new List<" + modelName + ">();");
                        commandText = "SELECT * FROM " + selectedTable + " WHERE " + ki.ColumnName + "=@" + ki.ColumnName + " ORDER BY " + primaryKey + " DESC";
                        method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
                        method.AppendLine(" var parameterList = new List<SqlParameter>();");
                        method.AppendLine(" var commandType = CommandType.Text;");
                        method.AppendLine(" parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + ki.ColumnName + "\", " + GeneralHelper.FirstCharacterToLower(ki.ColumnName) + "," + dataType + "));");
                        GetDataSetCodeText(method);
                        method.AppendLine("");
                    }
                }
                method.AppendLine("public " + staticText + " void Delete" + modelName + "(int " + GeneralHelper.FirstCharacterToLower(primaryKey) + ")");
                method.AppendLine(" {");
                commandText = "DELETE FROM " + selectedTable + " WHERE " + primaryKey + "=@" + primaryKey;
                method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
                method.AppendLine(" var parameterList = new List<SqlParameter>();");
                method.AppendLine(" var commandType = CommandType.Text;");
                method.AppendLine(" parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + primaryKey + "\", " + GeneralHelper.FirstCharacterToLower(primaryKey) + ",SqlDbType.Int));");
                method.AppendLine(" DatabaseUtility.ExecuteNonQuery(new SqlConnection(ConnectionString), commandText, commandType, parameterList.ToArray());");
                method.AppendLine(" }");
                method.AppendLine("");
                method.AppendLine(" public " + staticText + " " + modelName + " Get" + modelName + "(int " + primaryKey + ")");
                method.AppendLine(" {");
                commandText = "SELECT * FROM " + selectedTable + " WHERE " + primaryKey + "=@" + primaryKey;
                method.AppendLine(String.Format("String commandText = @\"{0}\";", commandText));
                method.AppendLine(" var parameterList = new List<SqlParameter>();");
                method.AppendLine(" var commandType = CommandType.Text;");
                method.AppendLine(" parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + primaryKey + "\", " + primaryKey + ",SqlDbType.Int));");
                method.AppendLine(" DataSet dataSet = DatabaseUtility.ExecuteDataSet(new SqlConnection(ConnectionString), commandText, commandType, parameterList.ToArray());");
                method.AppendLine(" if (dataSet.Tables.Count > 0)");
                method.AppendLine(" {");
                method.AppendLine(" using (DataTable dt = dataSet.Tables[0])");
                method.AppendLine(" {");
                method.AppendLine(" foreach (DataRow dr in dt.Rows)");
                method.AppendLine(" {");
                method.AppendLine(" var e = Get" + modifiedTableName + "FromDataRow(dr);");
                method.AppendLine(" return e;");
                method.AppendLine(" }");
                method.AppendLine(" }");
                method.AppendLine(" }");
                method.AppendLine(" return null;");
                method.AppendLine(" }");

                entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");
                method.AppendLine("public " + staticText + " int SaveOrUpdate" + modelName + "( " + modelName + " item)");
                method.AppendLine(" {");
                GetDatabaseUtilityParameters(DatabaseMetadata.SelectedTable.TableRowMetaDataList, method, entityPrefix + "SaveOrUpdate" + modifiedTableName, true);
                method.AppendLine(" int id = DatabaseUtility.ExecuteScalar(new SqlConnection(ConnectionString), commandText, commandType, parameterList.ToArray()).ToInt();");
                method.AppendLine(" return id;");
                method.AppendLine(" }");
                method.AppendLine(" }");
                method.AppendLine(" }");
                CodeGeneratorResult.SqlDatabaseOperation = method.ToString();
                _logger.LogTrace("CodeGeneratorResult.SqlDatabaseOperation:" + method.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                CodeGeneratorResult.SqlDatabaseOperation = ex.Message;
            }

        }
        private void GetMySqlDataSetCodeText(StringBuilder method)
        {
            method.AppendLine(
                " DataSet dataSet = MySqlHelper.ExecuteDataset(ConnectionString, commandText, parameterList.ToArray());");
            ConvertToDataMySqlTableToEntity(method);
        }

        private void ConvertToDataMySqlTableToEntity(StringBuilder method)
        {
            String modifiedTableName = CodeGeneratorResult.ModifiedTableName;
            method.AppendLine(" if (dataSet.Tables.Count > 0)");
            method.AppendLine(" {");
            method.AppendLine(" list = dataSet.Tables[0].ToList<" + modifiedTableName + ">();");
            method.AppendLine(" }");
            method.AppendLine(" return list;");
            method.AppendLine(" }");
        }

        private void GetDataSetCodeText(StringBuilder method)
        {
            method.AppendLine(
                " DataSet dataSet = DatabaseUtility.ExecuteDataSet(new SqlConnection(ConnectionString), commandText, commandType, parameterList.ToArray());");
            ConvertToDataTableToEntity(method);
        }

        private void ConvertToDataTableToEntity(StringBuilder method)
        {
            String modifiedTableName = CodeGeneratorResult.ModifiedTableName;
            method.AppendLine(" if (dataSet.Tables.Count > 0)");
            method.AppendLine(" {");
            method.AppendLine(" using (DataTable dt = dataSet.Tables[0])");
            method.AppendLine(" {");
            method.AppendLine(" foreach (DataRow dr in dt.Rows)");
            method.AppendLine(" {");
            method.AppendLine(" var e = Get" + modifiedTableName + "FromDataRow(dr);");
            method.AppendLine(" list.Add(e);");
            method.AppendLine(" }");
            method.AppendLine(" }");
            method.AppendLine(" }");
            method.AppendLine(" return list;");
            method.AppendLine(" }");
        }

        public void GenerateTableItem()
        {
            List<TableRowMetaData> linkedList = DatabaseMetadata.SelectedTable.TableRowMetaDataList;
            StringBuilder method = new StringBuilder();
            String realEntityName = CodeGeneratorResult.SelectedTable;
            String modelName = CodeGeneratorResult.ModifiedTableName;
            String modifiedTableName = CodeGeneratorResult.ModifiedTableName;
            string selectedTable = DatabaseMetadata.SelectedTable.TableNameWithSchema;
            String entityPrefix = GeneralHelper.GetEntityPrefixName(realEntityName);
            String primaryKey = TableRowMetaDataHelper.GetPrimaryKeys(linkedList);
            string primaryKeyOrginal = primaryKey;
            StringBuilder method2 = new StringBuilder();
            if (CodeGeneratorResult.IsMethodStatic)
            {
                method2.AppendLine("[Table(\"" + selectedTable + "\")]");
            }
            method2.AppendLine("public class " + modelName + "");
            method2.AppendLine("{");

            String testColumnName = "TestColumnName";
            method2.AppendLine(string.Format("// Entity annotions"));
            method2.AppendLine(string.Format("//[DataType(DataType.Text)]"));
            method2.AppendLine(string.Format("//[StringLength({0}, ErrorMessage = \"{1} cannot be longer than {0} characters.\")]", 100, testColumnName));
            method2.AppendLine(string.Format("//[Display(Name =\"{0}\")]", testColumnName));
            method2.AppendLine(string.Format("//[Required(ErrorMessage =\"{0}\")]", testColumnName));
            method2.AppendLine(string.Format("//[AllowHtml]"));

            foreach (TableRowMetaData item in linkedList)
            {
                try
                {

                    if (item.PrimaryKey && CodeGeneratorResult.IsModelAttributesVisible)
                    {
                        method2.AppendLine("[Key]");
                    }
                    if (CodeGeneratorResult.IsModelAttributesVisible)
                    {
                        //method2.AppendLine("[Required]");
                        method2.AppendLine(string.Format("[Display(Name =\"{0}\")]", item.ColumnName));
                        method2.AppendLine(string.Format("[Column(\"{0}\")]", item.ColumnName));

                        method2.AppendLine(string.Format("[Required(ErrorMessage =\"{0}\")]", item.ColumnName));
                    }



                    if (item.DataType.IndexOf("varchar") > -1 || item.DataType.IndexOf("text") > -1 || item.DataType.IndexOf("xml") > -1)
                    {
                        if (CodeGeneratorResult.IsModelAttributesVisible)
                        {
                            method2.AppendLine(string.Format("[DataType(DataType.Text)]"));
                            method2.AppendLine(string.Format("[StringLength({0}, ErrorMessage = \"{1} cannot be longer than {0} characters.\")]", item.DataTypeMaxChar, item.ColumnName));
                        }

                        method.AppendLine("public string " + item.ColumnName + " { get; set; }");
                        method2.AppendLine("public string " + item.ColumnName + " { get; set; }");
                    }
                    else if (item.DataType.IndexOf("int") > -1)
                    {
                        method.AppendLine("public int " + item.ColumnName + " { get; set; }");
                        method2.AppendLine("public int " + item.ColumnName + " { get; set; }");
                    }
                    else if (item.DataType.IndexOf("date") > -1)
                    {
                        if (CodeGeneratorResult.IsModelAttributesVisible)
                        {
                            method2.AppendLine(string.Format("[DataType(DataType.Date)]"));
                            method2.AppendLine(
                                string.Format(
                                    " [DisplayFormat(DataFormatString = \"{{0:yyyy/MM/dd}}\", ApplyFormatInEditMode = true)]"));

                        }
                        method.AppendLine("public DateTime " + item.ColumnName + " { get; set; }");
                        method2.AppendLine("public DateTime " + item.ColumnName + " { get; set; }");
                    }
                    else if (item.DataType.IndexOf("bit") > -1)
                    {
                        method.AppendLine("public Boolean " + item.ColumnName + " { get; set; }");
                        method2.AppendLine("public Boolean " + item.ColumnName + " { get; set; }");
                    }
                    else if (item.DataType.IndexOf("float") > -1)
                    {
                        method.AppendLine("public float " + item.ColumnName + " { get; set; }");
                        method2.AppendLine("public float " + item.ColumnName + " { get; set; }");
                    }
                    else if (item.DataType.IndexOf("char") > -1)
                    {
                        method.AppendLine("public char " + item.ColumnName + " { get; set; }");
                        method2.AppendLine("public char " + item.ColumnName + " { get; set; }");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }

            method2.AppendLine("public  " + modelName + "(){");
            method2.AppendLine("");
            method2.AppendLine("}");
            StringBuilder method555 = new StringBuilder();
            foreach (TableRowMetaData item in linkedList)
            {
                var fColumnName = GeneralHelper.FirstCharacterToLower(item.ColumnName);
                try
                {
                    if (item.DataType.IndexOf("varchar") > -1
                        || item.DataType.IndexOf("text") > -1
                        || item.DataType.IndexOf("xml") > -1
                        || item.DataType.IndexOf("nchar") > -1)
                    {
                        method555.Append("string " + fColumnName + ",");
                    }
                    else if (item.DataType.IndexOf("int") > -1)
                    {
                        method555.Append("int " + fColumnName + ",");
                    }
                    else if (item.DataType.IndexOf("date") > -1)
                    {
                        method555.Append("DateTime " + fColumnName + ",");
                    }
                    else if (item.DataType.IndexOf("bit") > -1)
                    {
                        method555.Append("Boolean " + fColumnName + ",");
                    }
                    else if (item.DataType.IndexOf("float") > -1)
                    {
                        method555.Append("float " + fColumnName + ",");
                    }
                    else if (item.DataType.IndexOf("char") > -1)
                    {
                        method555.Append("char " + fColumnName + ",");
                    }
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, ex.Message + "" + item.ColumnName);
                }


            }
            string m = String.Format("public {0} ({1})", modelName, method555.ToString().Trim().TrimEnd(','));
            method2.AppendLine(m + "{");
            method2.AppendLine("");
            foreach (TableRowMetaData item in linkedList)
            {
                try
                {
                    method2.AppendLine("this." + item.ColumnName + "=" + GeneralHelper.FirstCharacterToLower(item.ColumnName) + ";");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message + "" + item.ColumnName);
                }
            }
            method2.AppendLine("");
            method2.AppendLine("}");


            var fks = linkedList.Where(r => r.ForeignKey).ToList();
            foreach (var item in fks)
            {
                try
                {
                    var columnObj = GeneralHelper.getObject(item.ColumnName);
                    method2.AppendLine("private " + columnObj + " _" + columnObj.ToLower() + "=new " + columnObj + "();");
                    method2.AppendLine("public " + columnObj + " " + columnObj + "");
                    method2.AppendLine("{ ");
                    method2.AppendLine(" get {  return _" + columnObj.ToLower() + "; } ");
                    method2.AppendLine(" set {  _" + columnObj.ToLower() + "=value; } ");
                    method2.AppendLine("} ");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message + "" + item.ColumnName);
                }

            }
            method.AppendLine("");
            method.AppendLine("");
            method.AppendLine("");
            method2.AppendLine("");
            method2.AppendLine("");
            method2.AppendLine("public override string ToString() {");
            method2.AppendLine("return String.Format(");
            int i = 0;
            var method322 = new StringBuilder();
            var method32 = new StringBuilder();
            foreach (TableRowMetaData item in linkedList)
            {
                try
                {
                    method32.Append(String.Format("{0} ", item.ColumnName + ":{" + i + "}"));
                    method322.Append(String.Format("{0}, ", item.ColumnName));
                    i++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message + "" + item.ColumnName);
                }
            }
            method2.AppendLine(String.Format("\"{0}\",{1});", method32.ToString(), method322.ToString().Trim().TrimEnd(',')));
            method2.AppendLine("}");



            method2.AppendLine("}");


            CodeGeneratorResult.TableClassItem = method2.ToString();
            _logger.LogTrace("CodeGeneratorResult.TableClassItem:" + method2.ToString());
        }

        public string GenerateNewInstance()
        {
            List<TableRowMetaData> kontrolList = DatabaseMetadata.SelectedTable.TableRowMetaDataList;
            //GetBrouwerCollectionFromReader
            StringBuilder method = new StringBuilder();
            String staticText = CodeGeneratorResult.IsMethodStatic ? "static" : "";
            string databaseName = DatabaseMetadata.DatabaseName;
            String realEntityName = CodeGeneratorResult.SelectedTable;
            String modelName = CodeGeneratorResult.ModifiedTableName;
            String modifiedTableName = CodeGeneratorResult.ModifiedTableName;
            string selectedTable = DatabaseMetadata.SelectedTable.TableNameWithSchema;
            String entityPrefix = GeneralHelper.GetEntityPrefixName(realEntityName);
            String primaryKey = TableRowMetaDataHelper.GetPrimaryKeys(kontrolList);
            string primaryKeyOrginal = primaryKey;

            method.AppendLine("var item = new " + modelName + "();");
            method.AppendLine("");
            foreach (TableRowMetaData item in kontrolList)
            {

                if (item.DataType.IndexOf("varchar") > -1 || item.DataType.IndexOf("nchar") > -1)
                {
                    // method.AppendLine("item." + item.ColumnName + " = (read[\"" + item.ColumnName + "\"] is DBNull) ? \"\" : read[\"" + item.ColumnName + "\"].ToString();");
                    method.AppendLine("item." + item.ColumnName + " = \"\";");
                }
                else if (item.DataType.IndexOf("int") > -1)
                {
                    //method.AppendLine("item." + item.ColumnName + " = (read[\"" + item.ColumnName + "\"] is DBNull) ? -1 : Convert.ToInt32(read[\"" + item.ColumnName + "\"].ToString());");
                    method.AppendLine("item." + item.ColumnName + " = 1;");
                }
                else if (item.DataType.IndexOf("date") > -1)
                {
                    //method.AppendLine("item." + item.ColumnName + " = (read[\"" + item.ColumnName + "\"] is DBNull) ? DateTime.Now : DateTime.Parse(read[\"" + item.ColumnName + "\"].ToString());");
                    method.AppendLine("item." + item.ColumnName + " = DateTime.Now;");
                }
                else if (item.DataType.IndexOf("bit") > -1)
                {
                    //method.AppendLine("item." + item.ColumnName + " = (read[\"" + item.ColumnName + "\"] is DBNull) ? false : Boolean.Parse(read[\"" + item.ColumnName + "\"].ToString());");
                    method.AppendLine("item." + item.ColumnName + " = true;");
                }
                else if (item.DataType.IndexOf("float") > -1)
                {
                    //method.AppendLine("item." + item.ColumnName + " = (read[\"" + item.ColumnName + "\"] is DBNull) ? -1 : float.Parse(read[\"" + item.ColumnName + "\"].ToString());");
                    method.AppendLine("item." + item.ColumnName + " = 1;");
                }
                else
                {
                    // method.AppendLine("item." + item.ColumnName + " = (read[\"" + item.ColumnName + "\"] is DBNull) ? \"\" : read[\"" + item.ColumnName + "\"].ToString();");
                    method.AppendLine("item." + item.ColumnName + " = \"\";");
                }
            }


            method.AppendLine("");
            method.AppendLine("");
            method.AppendLine("");

            method.AppendLine(String.Format("public  class {0}Repository : GenericRepository<{2}Entities, {1}>, I{0}Repository", modelName, selectedTable, databaseName));
            method.AppendLine("{");
            method.AppendLine("}");

            method.AppendLine("");
            method.AppendLine("");

            method.AppendLine(String.Format("public interface I{0}Repository : IGenericRepository<{1}>", modelName, selectedTable));
            method.AppendLine("{");
            method.AppendLine("}");

            method.AppendLine("");
            method.AppendLine("");
            method.AppendLine("");

            StringBuilder method12 = new StringBuilder();


            method12.AppendLine("using GenericRepository.EntityFramework;");
            method12.AppendLine("namespace MyProject.Service.Repositories");
            method12.AppendLine("{");
            method12.AppendLine(String.Format("public  class {0}Repository : EntityRepository<{0}, int>, I{0}Repository", modelName));
            method12.AppendLine("{");
            method12.AppendLine(String.Format("private I{0}Context dbContext;", databaseName));
            method12.AppendLine(String.Format("public {1}Repository(I{0}Context dbContext) : base(dbContext)", databaseName, modelName));
            method12.AppendLine("{");
            method12.AppendLine("    this.dbContext = dbContext;");
            method12.AppendLine("}");
            method12.AppendLine("}");
            method12.AppendLine("}");

            method12.AppendLine("");
            method12.AppendLine("");


            StringBuilder method11 = new StringBuilder();
            method11.AppendLine("using GenericRepository.EntityFramework;");
            method11.AppendLine("namespace MyProject.Service.Repositories.Interfaces");
            method11.AppendLine("{");
            method11.AppendLine(String.Format("public interface I{0}Repository : IEntityRepository<{0}, int>", modelName));
            method11.AppendLine("{");
            method11.AppendLine("}");
            method11.AppendLine("}");

            // createFile(method, String.Format("{0}Repository", modelName));

            method.AppendLine(method11.ToString());
            method.AppendLine(method12.ToString());


            CodeGeneratorResult.TableClassInstance = method.ToString();

            return method.ToString();

        }

        public void GenerateTableServices()
        {
            List<TableRowMetaData> linkedList = DatabaseMetadata.SelectedTable.TableRowMetaDataList;
            StringBuilder method = new StringBuilder();
            String realEntityName = CodeGeneratorResult.SelectedTable;
            String modelName = CodeGeneratorResult.ModifiedTableName;
            String modifiedTableName = CodeGeneratorResult.ModifiedTableName;
            String nameSpace = CodeGeneratorResult.NameSpace.ToStr(DEFAULT_NAMESPACE);
            String entityPrefix = GeneralHelper.GetEntityPrefixName(realEntityName);
            String primaryKey = TableRowMetaDataHelper.GetPrimaryKeys(linkedList);
            string primaryKeyOrginal = primaryKey;
            primaryKey = GeneralHelper.FirstCharacterToLower(primaryKey);
            String staticText = CodeGeneratorResult.IsMethodStatic ? "static" : "";
            String repositoryName = CodeGeneratorResult.IsMethodStatic ?
                String.Format("{0}Repository", modelName) :
                String.Format("_{0}Repository", GeneralHelper.FirstCharacterToLower(modelName));

            method.AppendLine(String.Format("using {0}.DB;", nameSpace));
            method.AppendLine(String.Format("using {0}.Entities;", nameSpace));
            method.AppendLine(String.Format("using {0}.Repositories;", nameSpace));
            method.AppendLine("using System;");
            method.AppendLine("using System.Linq;");
            method.AppendLine("using System.Runtime.Caching;");
            method.AppendLine("using System.Text;");
            method.AppendLine("using System.NLog;");
            method.AppendLine("using System.Threading.Tasks;");
            method.AppendLine("");
            method.AppendLine(String.Format("namespace {0}.Services", nameSpace));
            method.AppendLine("{");
            method.AppendLine(String.Format("public class {0}Service", modelName.Replace("Nwm", "")));
            method.AppendLine("{");
            method.AppendLine("private static readonly Logger Logger = LogManager.GetCurrentClassLogger();");
            method.AppendLine("private static string CacheKeyAllItems = \"" + modelName + "Cache\";");
            method.AppendLine("");
            if (!CodeGeneratorResult.IsMethodStatic)
            {
                method.AppendLine(String.Format("private {1}Repository _{0}Repository;", GeneralHelper.FirstCharacterToLower(modelName), modelName));
                method.AppendLine(String.Format("public {0}Service()", modelName.Replace("Nwm", "")));
                method.AppendLine("{");
                method.AppendLine(String.Format("_{0}Repository=new {1}Repository();", GeneralHelper.FirstCharacterToLower(modelName), modelName));
                method.AppendLine("}");
            }

            method.AppendLine("");
            method.AppendLine("public " + staticText + " List<" + modelName + "> Get" + modelName + "sFromCache()");
            method.AppendLine("{");
            method.AppendLine("var items = (List<" + modelName + ">)MemoryCache.Default.Get(CacheKeyAllItems);");
            method.AppendLine("if (items == null)");
            method.AppendLine("{");
            method.AppendLine("items = Get" + modelName + "s();");
            method.AppendLine(" CacheItemPolicy policy = null;");
            method.AppendLine("policy = new CacheItemPolicy();");
            method.AppendLine("policy.Priority = CacheItemPriority.Default;");
            method.AppendLine(" policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(Settings.CacheMediumSeconds);");
            method.AppendLine("MemoryCache.Default.Set(CacheKeyAllItems, items, policy);");
            method.AppendLine("}");
            method.AppendLine(" return items;");
            method.AppendLine("}");
            method.AppendLine("");
            method.AppendLine("public " + staticText + " List<" + modelName + "> Get" + modelName + "s()");
            method.AppendLine("{");
            method.AppendLine("      var " + modelName.ToLower() + "Result = new  List <" + modelName + ">();");
            method.AppendLine("try");
            method.AppendLine("{");
            method.AppendLine("      " + modelName.ToLower() + "Result = " + repositoryName + ".Get" + modelName + "s();");
            method.AppendLine("}catch(Exception ex)");
            method.AppendLine("{");
            method.AppendLine("_logger.LogError(ex, ex.Message);");
            method.AppendLine("}");
            method.AppendLine("      return " + modelName.ToLower() + "Result;");
            method.AppendLine("}");
            method.AppendLine("public " + staticText + " int SaveOrUpdate" + modelName + "( " + modelName + " item)");
            method.AppendLine("{");
            method.AppendLine("try");
            method.AppendLine("{");
            method.AppendLine("     RemoveCache();");
            method.AppendLine("     return " + repositoryName + ".SaveOrUpdate" + modelName + "(item);");
            method.AppendLine("}catch(Exception ex)");
            method.AppendLine("{");
            method.AppendLine("_logger.LogError(ex, ex.Message);");
            method.AppendLine("}");
            method.AppendLine("      return -1;");
            method.AppendLine("}");
            method.AppendLine("public " + staticText + " " + modelName + " Get" + modelName + "(int " + primaryKey + ")");
            method.AppendLine("{");
            method.AppendLine("      var item = new  " + modelName + "();");
            method.AppendLine("try");
            method.AppendLine("{");
            method.AppendLine("item = Get" + modelName + "sFromCache().FirstOrDefault(r => r." + primaryKeyOrginal + " == " + primaryKey + ");");
            method.AppendLine("if (item != null) return item;");
            method.AppendLine("     item = " + repositoryName + ".Get" + modelName + "(" + primaryKey + ");");
            method.AppendLine("}catch(Exception ex)");
            method.AppendLine("{");
            method.AppendLine("_logger.LogError(ex, ex.Message);");
            method.AppendLine("}");
            method.AppendLine("      return item;");
            method.AppendLine("}");
            method.AppendLine("public " + staticText + " void Delete" + modelName + "(int " + primaryKey + ")");
            method.AppendLine("{");
            method.AppendLine("try");
            method.AppendLine("{");
            method.AppendLine("     RemoveCache();");
            method.AppendLine("     " + repositoryName + ".Delete" + modelName + "(" + primaryKey + ");");
            method.AppendLine("}catch(Exception ex)");
            method.AppendLine("{");
            method.AppendLine("_logger.LogError(ex, ex.Message);");
            method.AppendLine("}");
            method.AppendLine("}");
            method.AppendLine("public " + staticText + " void RemoveCache()");
            method.AppendLine("{");
            method.AppendLine("     MemoryCache.Default.Remove(CacheKeyAllItems);");
            method.AppendLine("}");
            foreach (var ki in linkedList)
            {
                if (ki.ForeignKey)
                {
                    //String dataType = GetSqlDataTypeFromColumnDataType(ki);
                    String cSharpType = TableRowMetaDataHelper.GetCSharpDataType(ki);
                    method.AppendLine("//" + ki.ColumnName);
                    method.AppendLine("public " + staticText + "  List<" + modelName + "> Get" + modelName + "By" + ki.ColumnName + "(" + cSharpType + " " + GeneralHelper.FirstCharacterToLower(ki.ColumnName) + ")");
                    method.AppendLine("{");
                    method.AppendLine("   return  " + repositoryName + ".Get" + modelName + "By" + ki.ColumnName + "(" + GeneralHelper.FirstCharacterToLower(ki.ColumnName) + ");");
                    method.AppendLine("}");
                }
            }
            method.AppendLine("}");
            method.AppendLine("}");
            CodeGeneratorResult.TableRepository = method.ToStr().TrimStart().TrimEnd();
        }



        private void GetDatabaseUtilityParameters(List<TableRowMetaData> kontrolList, StringBuilder method, String commandText = "", bool isSp = false)
        {
            String realEntityName = CodeGeneratorResult.SelectedTable;

            method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
            method.AppendLine(" var parameterList = new List<SqlParameter>();");
            method.AppendLine(!isSp ? "var commandType = CommandType.Text;" : "var commandType = CommandType.StoredProcedure;");


            foreach (TableRowMetaData item in kontrolList)
            {
                var sqlParameter = GeneralHelper.GetUrlString(item.ColumnName);
                if (item.DataType.IndexOf("xml") > -1)
                {
                    method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." +
                                      item.ColumnName + ".ToStr(),SqlDbType.Xml));");
                }
                else if (item.DataType.IndexOf("varchar") > -1 || item.DataType.IndexOf("nchar") > -1)
                {
                    method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." +
                                      item.ColumnName + ".ToStr(),SqlDbType.NVarChar));");
                }
                else if (item.DataType.IndexOf("int") > -1)
                {
                    method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." +
                                      item.ColumnName + ",SqlDbType.Int));");
                }
                else if (item.DataType.IndexOf("date") > -1)
                {
                    method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." +
                                      item.ColumnName + ",SqlDbType.DateTime));");
                }
                else if (item.DataType.IndexOf("bit") > -1)
                {
                    method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." +
                                      item.ColumnName + ",SqlDbType.Bit));");
                }
                else if (item.DataType.IndexOf("float") > -1)
                {
                    method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." +
                                      item.ColumnName + ",SqlDbType.Float));");
                }
                else
                {
                    method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." +
                                      item.ColumnName + ",SqlDbType.NVarChar));");
                }

            }
        }


        public void GenerateStoredProcExecutionCode()
        {

            try
            {
                #region Execute SP to get tables so that we can generate code
                string StoredProc_Exec = CodeGeneratorResult.StoredProcExecCode.ToStr().Trim();

                if (String.IsNullOrEmpty(StoredProc_Exec))
                {
                    return;
                }
                StoredProc_Exec = StoredProc_Exec.Replace("\r\n", " ").Trim();
                string returnResultClass = "NwmResultItem";
                string storedProcName = "";
                DataSet ds = null;
                String sqlCommand = "";
                List<string> tableNames = new List<string>();
                try
                {
                    storedProcName = storedProcName.SplitString().FirstOrDefault();
                    String[] storedProcNameParts = storedProcName.SplitString(@"_").ToArray();
                    storedProcName = storedProcNameParts != null && storedProcNameParts.Any() ? storedProcNameParts[1] : storedProcName;
                    storedProcName = GeneralHelper.ToTitleCase(storedProcName);
                    //  storedProcName = StoredProc_Exec.Split("-".ToCharArray());
                    StoredProc_Exec = StoredProc_Exec.Replace("]", "").Replace("[", "").Trim();
                    string[] m = StoredProc_Exec.Split("-".ToCharArray());

                    sqlCommand = m.FirstOrDefault();

                    ds = TableService.GetDataSet(sqlCommand, DatabaseMetadata.ConnectionString);

                    String tableNamesTxt = m.LastOrDefault();

                    #region Entity names are coming from user input
                    // If no entity names are defined, we will generate table names
                    if (m.Length > 1)
                    {
                        if (!String.IsNullOrEmpty(tableNamesTxt))
                        {
                            tableNames = Regex.Split(tableNamesTxt, @"\s+").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                        }

                        // we have more than one tables coming from SP
                        if (ds.Tables.Count > 1)
                        {
                            // The last table names is the result of that method.
                            // Table names should be more than number of returned table
                            // 
                            if (ds.Tables.Count + 1 == tableNames.Count)
                            {
                                returnResultClass = tableNames.LastOrDefault();
                            }
                            else if (ds.Tables.Count == tableNames.Count)
                            {

                            }
                            else if (ds.Tables.Count > tableNames.Count)
                            {
                                int diff = ds.Tables.Count - tableNames.Count;
                                for (int i = 0; i < diff; i++)
                                {
                                    tableNames.Add("Tablo" + i);
                                }
                            }
                            else if (ds.Tables.Count < tableNames.Count)
                            {
                                // number to remove is the difference between the current length
                                // and the maximum length you want to allow.
                                var count = tableNames.Count - ds.Tables.Count;
                                if (count > 0)
                                {
                                    // remove that number of items from the start of the list
                                    tableNames.RemoveRange(0, count);
                                }
                            }
                        }

                    }
                    else
                    {
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            tableNames.Add("Tablo" + i);
                        }

                    }
                    #endregion

                }
                catch (Exception ex)
                {

                    CodeGeneratorResult.StoredProcExec = ex.StackTrace;


                }
                if (ds == null)
                {
                    return;
                }
                #endregion
                #region Generating ENTITY FROM datable coming from SP
                try
                {

                    var built2 = new StringBuilder();
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        DataTable table = ds.Tables[i];

                        var built = new StringBuilder();
                        built.AppendLine(String.Format("public class {0} ", tableNames.Any() ? tableNames[i] : "Tablo" + i) + "{");
                        foreach (DataColumn column in table.Columns)
                        {
                            try
                            {
                                String dataType = "string";
                                DataRow firstRow = table.Rows.Cast<DataRow>().ToArray().Take(1).FirstOrDefault();
                                if (firstRow != null)
                                {

                                    dataType = firstRow[column].GetType().Name.ToLower()
                                        .Replace("32", "")
                                        .Replace("boolean", "bool")
                                        .Replace("datetime", "DateTime");
                                    if (firstRow[column].GetType().Name.Equals("DBNull"))
                                    {
                                        dataType = "string";
                                    }
                                }

                                built.AppendLine(String.Format("public {1} {0} ", column.ColumnName, dataType) + "{ get; set;}");
                            }
                            catch (Exception ee)
                            {

                            }

                        }
                        built.AppendLine("}");
                        built2.AppendLine(built.ToString());

                    }
                    if (ds.Tables.Count == 1)
                    {
                        CodeGeneratorResult.StoredProcExecModel = built2.ToString();
                    }
                    else
                    {
                        built2.AppendLine("");
                        //Generating the return result class and its related list classess 
                        built2.AppendLine(String.Format("public class {0} ", returnResultClass) + "{");
                        for (int i = 0; i < tableNames.Count; i++)
                        {
                            if (tableNames[i].Equals(returnResultClass, StringComparison.InvariantCultureIgnoreCase))
                                continue;
                            built2.AppendLine(String.Format("public List<{1}> {0}List ", tableNames[i], tableNames[i]) + "{ get; set;}");
                        }
                        built2.AppendLine("}");
                        CodeGeneratorResult.StoredProcExecModel = built2.ToString();
                    }

                }
                catch (Exception ex)
                {
                    CodeGeneratorResult.StoredProcExecModel = ex.StackTrace;

                }
                #endregion

                #region  Generating Table to Entity method code
                String staticText = CodeGeneratorResult.IsMethodStatic ? "static" : "";
                try
                {
                    var built2 = new StringBuilder();
                    // generating entities from data row classes 
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        DataTable table = ds.Tables[i];
                        String modelName = String.Format("{0}", tableNames.Any() ? tableNames[i] : "Tablo" + i);
                        var method = new StringBuilder();
                        method.AppendLine("private " + staticText + " " + modelName + " Get" + modelName + "FromDataRow(DataRow dr)");
                        method.AppendLine("{");
                        method.AppendLine("var item = new " + modelName + "();");
                        method.AppendLine("");

                        foreach (DataColumn column in table.Columns)
                        {
                            String dataType = "string";
                            DataRow firstRow = table.Rows.Cast<DataRow>().ToArray().Take(1).FirstOrDefault();
                            if (firstRow != null)
                            {

                                dataType = firstRow[column].GetType().Name.ToLower().Replace("32", "").Replace("boolean", "bool").Replace("datetime", "DateTime");
                                if (firstRow[column].GetType().Name.Equals("DBNull"))
                                {
                                    dataType = "string";
                                }
                            }

                            dataType = dataType.ToLower();
                            // method.AppendLine("item." + column.ColumnName + " = dr[\"" + column.ColumnName + "\"].ToStr();");


                            if (dataType.IndexOf("string") > -1)
                            {
                                // method.AppendLine("item." + item.ColumnName + " = (read[\"" + item.ColumnName + "\"] is DBNull) ? \"\" : read[\"" + item.ColumnName + "\"].ToString();");
                                method.AppendLine("item." + column.ColumnName + " = dr[\"" + column.ColumnName + "\"].ToStr();");
                            }
                            else if (dataType.IndexOf("int") > -1)
                            {
                                //method.AppendLine("item." + item.ColumnName + " = (read[\"" + item.ColumnName + "\"] is DBNull) ? -1 : Convert.ToInt32(read[\"" + item.ColumnName + "\"].ToString());");
                                method.AppendLine("item." + column.ColumnName + " = dr[\"" + column.ColumnName + "\"].ToInt();");
                            }
                            else if (dataType.IndexOf("date") > -1)
                            {
                                //method.AppendLine("item." + item.ColumnName + " = (read[\"" + item.ColumnName + "\"] is DBNull) ? DateTime.Now : DateTime.Parse(read[\"" + item.ColumnName + "\"].ToString());");
                                method.AppendLine("item." + column.ColumnName + " = dr[\"" + column.ColumnName + "\"].ToDateTime();");

                            }
                            else if (dataType.IndexOf("bool") > -1)
                            {
                                //method.AppendLine("item." + item.ColumnName + " = (read[\"" + item.ColumnName + "\"] is DBNull) ? false : Boolean.Parse(read[\"" + item.ColumnName + "\"].ToString());");
                                method.AppendLine("item." + column.ColumnName + " = dr[\"" + column.ColumnName + "\"].ToBool();");
                            }
                            else if (dataType.IndexOf("float") > -1)
                            {
                                //method.AppendLine("item." + item.ColumnName + " = (read[\"" + item.ColumnName + "\"] is DBNull) ? -1 : float.Parse(read[\"" + item.ColumnName + "\"].ToString());");
                                method.AppendLine("item." + column.ColumnName + " = dr[\"" + column.ColumnName + "\"].ToFloat();");
                            }

                        }
                        method.AppendLine("return item;");
                        method.AppendLine("}");
                        built2.AppendLine(method.ToString());

                    }

                    CodeGeneratorResult.StoredProcExecModelDataReader = built2.ToString();





                }
                catch (Exception ex)
                {
                    CodeGeneratorResult.StoredProcExecModelDataReader = ex.StackTrace;

                }
                #endregion
                #region Generationg  calling SP method, main functionality

                try
                {
                    String modelName2 = "";
                    string returnTypeText = "";
                    if (ds.Tables.Count > 1)
                    {
                        returnTypeText = "dbSpResult";
                    }

                    var method = new StringBuilder();
                    method.AppendLine("//" + StoredProc_Exec);
                    var queryParts = Regex.Split(sqlCommand, @"\s+").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                    String sp = queryParts.FirstOrDefault();
                    sqlCommand = sqlCommand.Replace(sp, "");

                    var queryParts2 = Regex.Split(sqlCommand, @",").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();



                    String modelName = String.Format("{0}", tableNames.Any() ? tableNames.LastOrDefault() : "Table" + (ds.Tables.Count + 1));
                    String returnOfMethodName = tableNames.Any() && tableNames.Count > 1 ? returnResultClass : " List<" + modelName + ">";
                    String selectedTable = DatabaseMetadata.DatabaseName;
                    string methodParameterBuiltText = "()";
                    if (queryParts2.Any())
                    {
                        StringBuilder methodParameterBuilt = new StringBuilder();

                        methodParameterBuilt.Append("(");
                        foreach (var item in queryParts2)
                        {
                            try
                            {
                                var parameterParts = Regex.Split(item, @"=").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                                var paraterValue = parameterParts.LastOrDefault();
                                var paramterName = parameterParts.FirstOrDefault().Replace("@", "");
                                var parameterName2 = paramterName.ToLower();
                                if (paramterName.ToLower().Contains("date"))
                                {
                                    methodParameterBuilt.Append("DateTime ? " + parameterName2 + " =null,");
                                }
                                else if (paraterValue.Contains("'"))
                                {
                                    paraterValue = paraterValue.Replace("'", "\"");
                                    methodParameterBuilt.Append("string " + parameterName2 + " = " + paraterValue + ",");
                                }
                                else
                                {
                                    methodParameterBuilt.Append("int " + parameterName2 + " = " + paraterValue + ",");
                                }


                            }
                            catch (Exception)
                            {


                            }

                        }
                        methodParameterBuiltText = methodParameterBuilt.ToString().Trim().TrimEnd(",".ToCharArray());
                        methodParameterBuiltText = methodParameterBuiltText + ")";
                    }

                    method.AppendLine(" public " + staticText + " " + returnOfMethodName + " Get" + storedProcName + methodParameterBuiltText.ToString());
                    method.AppendLine(" {");
                    String commandText = sp;
                    method.AppendLine(" string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;");
                    method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
                    method.AppendLine(" var parameterList = new List<SqlParameter>();");
                    method.AppendLine(" var commandType = CommandType.StoredProcedure;");


                    foreach (var item in queryParts2)
                    {
                        try
                        {
                            var parameterParts = Regex.Split(item, @"=").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                            var paraterValue = parameterParts.LastOrDefault();
                            var paramterName = parameterParts.FirstOrDefault().Replace("@", "");
                            var parameterName2 = paramterName.ToLower();
                            string sqlDbType = "SqlDbType.Int";
                            if (paramterName.ToLower().Contains("date"))
                            {
                                method.AppendLine("if(" + parameterName2 + ".HasValue)");
                                sqlDbType = "SqlDbType.DateTime";
                            }
                            else if (paraterValue.Contains("'"))
                            {
                                sqlDbType = "SqlDbType.NVarChar";
                                parameterName2 = parameterName2 + ".ToStr()";
                            }
                            else
                            {
                                //    parameterName2 = parameterName2;
                            }

                            method.AppendLine(" parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + paramterName + "\", " + parameterName2 + "," + sqlDbType + "));");

                        }
                        catch (Exception)
                        {


                        }

                    }
                    if (ds.Tables.Count == 1)
                    {
                        method.AppendLine(String.Format("[return_type]"));
                    }
                    else
                    {
                        method.AppendLine(String.Format("var dbSpResult=new {0}();", returnResultClass));
                    }

                    method.AppendLine(" DataSet dataSet = DatabaseUtility.ExecuteDataSet(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray());");
                    method.AppendLine(" if (dataSet.Tables.Count > 0)");
                    method.AppendLine(" {");

                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        try
                        {
                            modelName2 = String.Format("{0}", tableNames.Any() ? tableNames[i] : "Tablo" + i);
                            if (ds.Tables.Count != 1)
                            {
                                method.AppendLine(String.Format("var list{0}=new List<{1}>();", i, modelName2));
                            }
                            else
                            {

                            }
                            method.AppendLine(String.Format(" using (DataTable dt = dataSet.Tables[{0}])", i));
                            method.AppendLine(" {");
                            method.AppendLine(" foreach (DataRow dr in dt.Rows)");
                            method.AppendLine(" {");
                            method.AppendLine(String.Format(" var e = Get{0}FromDataRow(dr);", modelName2));
                            method.AppendLine(String.Format(" list{0}.Add(e);", i));
                            method.AppendLine(" }");
                            if (ds.Tables.Count > 1)
                            {
                                method.AppendLine(" dbSpResult." + modelName2 + "List=list" + i + ";");
                            }

                            method.AppendLine(" }");
                            method.AppendLine(" ");
                            method.AppendLine(" ");
                        }
                        catch (Exception)
                        {


                        }

                    }
                    returnTypeText = String.Format("var list{0}=new List<{1}>();", 0, modelName2);

                    method.Replace("[return_type]", returnTypeText);
                    method.AppendLine(" }");
                    if (ds.Tables.Count > 1)
                    {
                        method.AppendLine(" return dbSpResult;");
                    }
                    else
                    {
                        method.AppendLine(" return list0;");
                    }



                    method.AppendLine(" }");

                    CodeGeneratorResult.StoredProcExec = method.ToString();
                }
                catch (Exception ex)
                {
                    CodeGeneratorResult.StoredProcExec = ex.StackTrace;
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public void GenerateAspMvcControllerClass()
        {
            List<TableRowMetaData> kontrolList = DatabaseMetadata.SelectedTable.TableRowMetaDataList;
            String realEntityName = CodeGeneratorResult.SelectedTable;
            String modelName = CodeGeneratorResult.ModifiedTableName;
            String modifiedTableName = CodeGeneratorResult.ModifiedTableName.ToStr();
            String nameSpace = CodeGeneratorResult.NameSpace.ToStr(DEFAULT_NAMESPACE);
            string selectedTable = DatabaseMetadata.SelectedTable.TableNameWithSchema;
            String entityPrefix = GeneralHelper.GetEntityPrefixName(realEntityName);
            String primaryKey = TableRowMetaDataHelper.GetPrimaryKeys(kontrolList);
            string primaryKeyOrginal = primaryKey;
            String staticText = CodeGeneratorResult.IsMethodStatic ? "static" : "";

            var built = new StringBuilder();



            built.AppendLine("using System;");
            built.AppendLine("using System.Collections.Generic;");
            built.AppendLine("using System.Linq;");
            built.AppendLine("using System.Web;");
            built.AppendLine("using System.Web.Mvc;");
            built.AppendLine("using NLog;");
            built.AppendLine(String.Format("using {0}.Entities;", nameSpace));
            built.AppendLine(String.Format("using {0}.Helpers;", nameSpace));
            built.AppendLine(String.Format("using {0}.Services;", nameSpace));
            built.AppendLine(" ");
            built.AppendLine(" ");

            built.AppendLine(String.Format("namespace {0}.Controllers", nameSpace));
            built.AppendLine(" {");
            built.AppendLine(" public class HomeController : Controller");
            built.AppendLine(" {");
            built.AppendLine(" private static readonly Logger Logger = LogManager.GetCurrentClassLogger();");

            built.AppendLine("//[OutputCache(CacheProfile = \"Cache1Hour\")]");
            built.AppendLine("public ActionResult Index()");
            built.AppendLine("{");
            built.AppendLine(String.Format("var items = {0}Repository.Get{1}s();", modelName.Replace("Nwm", ""), modelName));
            built.AppendLine("return View(items);");
            built.AppendLine("}");

            built.AppendLine("//[OutputCache(CacheProfile = \"Cache1Hour\")]");
            built.AppendLine(String.Format("public ActionResult {0}Detail(String id)", modelName));
            built.AppendLine("{");
            built.AppendLine(String.Format("int {0} = id.Split('-').Last().ToInt();", primaryKey.ToLower()));
            built.AppendLine(String.Format("var {0} = {1}Repository.Get{3}({2});", modelName.ToLower(), modelName.Replace("Nwm", ""), primaryKey.ToLower(), modelName));
            built.AppendLine(String.Format("return View({0});", modelName.ToLower()));
            built.AppendLine("}");


            built.AppendLine(String.Format("public ActionResult SaveOrUpdate{0}(int id)", modelName));
            built.AppendLine("{");
            built.AppendLine(String.Format("int {0} = id;", primaryKey.ToLower()));
            built.AppendLine(String.Format("var {0} = new {1}();", modelName.ToLower(), modelName));
            built.AppendLine(String.Format("if({0} == 0)", primaryKey.ToLower()));
            built.AppendLine("{");
            built.AppendLine("}else{");
            built.AppendLine(String.Format("{0} = {1}Repository.Get{3}({2});", modelName.ToLower(), modelName.Replace("Nwm", ""),
                primaryKey.ToLower(), modelName));
            built.AppendLine("}");
            built.AppendLine(String.Format("return View({0});", modelName.ToLower()));
            built.AppendLine("}");

            built.AppendLine("[HttpPost]");
            built.AppendLine(String.Format("public ActionResult SaveOrUpdate{0}({0} {1})", modelName, modelName.ToLower()));
            built.AppendLine("{");
            built.AppendLine(String.Format("int {0} = {1}Repository.SaveOrUpdate{3}({2});", primaryKey.ToLower(), modelName.Replace("Nwm", ""), modelName.ToLower(), modelName));
            built.AppendLine(String.Format("return RedirectToAction(\"Index\");"));
            built.AppendLine("}");

            built.AppendLine(String.Format("public ActionResult Delete{0}(int id)", modelName));
            built.AppendLine("{");
            built.AppendLine(String.Format("int {0} = id;", GeneralHelper.FirstCharacterToLower(primaryKey)));
            built.AppendLine(String.Format("{0}Repository.Delete{2}({1});", modelName.Replace("Nwm", ""), GeneralHelper.FirstCharacterToLower(primaryKey), modelName));
            built.AppendLine(String.Format("return RedirectToAction(\"Index\");"));
            built.AppendLine("}");

            built.AppendLine("}");
            built.AppendLine("}");

            CodeGeneratorResult.AspMvcControllerClass = built.ToString();
        }

        public void GenerateMergeSqlStoredProcedure()
        {

            StringBuilder built = new StringBuilder();

            List<TableRowMetaData> list = DatabaseMetadata.SelectedTable.TableRowMetaDataList;
            TableRowMetaData prKey = TableRowMetaDataHelper.GetPrimaryKeysObj(list);
            try
            {
                String realEntityName = CodeGeneratorResult.SelectedTable;
                String modelName = CodeGeneratorResult.ModifiedTableName;
                String modifiedTableName = CodeGeneratorResult.ModifiedTableName;
                string selectedTable = DatabaseMetadata.SelectedTable.TableNameWithSchema;
                String entityPrefix = GeneralHelper.GetEntityPrefixName(realEntityName);
                String primaryKey = TableRowMetaDataHelper.GetPrimaryKeys(list);
                string primaryKeyOrginal = primaryKey;
                String staticText = CodeGeneratorResult.IsMethodStatic ? "static" : "";

                entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");



                built = new StringBuilder();
                built.AppendLine("CREATE PROCEDURE  " + entityPrefix + "Merge" + modifiedTableName + "(");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    built.AppendLine("@" + GeneralHelper.GetUrlString(item.ColumnName) + " " + item.DataTypeMaxChar + " = " + (String.IsNullOrEmpty(item.ColumnDefaultValue) ? "NULL" : item.ColumnDefaultValue) + comma);
                }

                built.Append(")");
                built.AppendLine("AS");
                built.AppendLine("BEGIN");
                built.AppendLine("DECLARE @Output TABLE ( ActionType NVARCHAR(20)," +
                    " SourcePrimaryKey  INT NOT NULL --PRIMARY KEY NONCLUSTERED");
                built.AppendLine(");");
                built.AppendLine("MERGE " + selectedTable + " TRGT  ");
                built.AppendLine("USING (");
                built.AppendLine("    SELECT ");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                        built.AppendLine(String.Format("@{0} {0} {1}",GeneralHelper.GetUrlString(item.ColumnName), comma));
                }

                built.AppendLine(") SRC ");
                built.AppendLine(" ON TRGT." + primaryKey+"=SRC."+ primaryKey);
                built.AppendLine(" WHEN NOT MATCHED BY TARGET THEN INSERT (");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    if (!item.PrimaryKey)
                        built.AppendLine(item.ColumnName + comma);
                }

                built.AppendLine(")");
                built.AppendLine("VALUES (");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    if (!item.PrimaryKey)
                    {
                        built.AppendLine("SRC."+GeneralHelper.GetUrlString(item.ColumnName) + comma);
                    }
                }
                built.AppendLine(")");
                built.AppendLine("WHEN MATCHED AND ");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var OR = (i != (list.Count - 1) ? " OR " : "");
                    if (!item.PrimaryKey)
                    {
                        built.AppendLine(String.Format("TRGT.{0}", item.ColumnName) + " <> SRC." + GeneralHelper.GetUrlString(item.ColumnName)+ OR);
                    }
                }
                built.AppendLine("THEN UPDATE SET");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    if (!item.PrimaryKey)
                    {
                        built.AppendLine(String.Format("[{0}]", item.ColumnName) + " = SRC." + GeneralHelper.GetUrlString(item.ColumnName) + comma);
                    }
                }
                built.AppendLine("--WHEN NOT MATCHED BY SOURCE THEN ");
                built.AppendLine("--DELETE");
                built.AppendLine(" OUTPUT $action,");
                built.AppendLine(" INSERTED."+primaryKey+ " AS " + primaryKey + " INTO @Output;");
                built.AppendLine(" SELECT TOP 1 SourcePrimaryKey from @Output");
                built.AppendLine(" END");
                built.AppendLine(" GO");
                CodeGeneratorResult.MergeSqlStoredProcedure = built.ToString();
                _logger.LogTrace("CodeGeneratorResult.GenerateMergeSqlStoredProcedure:" + built.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                CodeGeneratorResult.MergeSqlStoredProcedure = ex.Message;
            }
        }
    }
}

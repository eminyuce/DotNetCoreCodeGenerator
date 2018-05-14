using DotNetCodeGenerator.Domain.Entities;
using DotNetCodeGenerator.Domain.Entities.Enums;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNetCodeGenerator.Domain.Helpers
{
    public class SqlParserHelper : ISqlParserHelper
    {

        private readonly ILogger _logger;

        public SqlParserHelper(ILogger<SqlParserHelper> logger)
        {
            _logger = logger;
        }


        private string RemoveBrackets(String m)
        {
            return m.Replace("[", "").Replace("]", "");
        }
        private  string RemoveParatheses(String m)
        {
            return m.Replace("(", "").Replace(")", "");
        }
        private  string RemoveNail(String m)
        {
            return m.Replace("`", "");
        }
        public  DatabaseMetadata ParseSqlCreateStatement(string txt = "")
        {
            DatabaseMetadata databaseMetaData = new DatabaseMetadata();
            string tableName = "";
            string primaryKey = "";
            var bracketsRegex = @"\[(.*?)\]";
            var paranthesesRegex = @"\((.*?)\)";
            var mysqlQuoteRegex = @"\`(.*?)\`";
            DatabaseType databaseType = DatabaseType.UnDefined;

            String databaseName = "UNKNOWN";
            databaseMetaData.DatabaseName = databaseName;
            var tableMetaDataList = new List<TableRowMetaData>();
            databaseMetaData.SelectedTable = new TableMetaData();

            databaseMetaData.SelectedTable.TableRowMetaDataList = tableMetaDataList;
            int counter = 0;

            var textLines = new List<String>();
            // First determine database type
            using (StringReader reader = new StringReader(txt))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!String.IsNullOrEmpty(line.Trim()))
                    {
                        textLines.Add(line);
                        string lineLower = line.ToLower();
                        try
                        {
                            if (lineLower.IndexOf("identity") > -1 || lineLower.IndexOf("pad_index") > -1 || lineLower.IndexOf("primary key clustered") > -1)
                            {
                                databaseType = DatabaseType.MsSql;
                            }
                            else if (lineLower.IndexOf("auto_increment") > -1 || lineLower.IndexOf("primary key") > -1 || lineLower.IndexOf("engine") > -1)
                            {
                                databaseType = DatabaseType.MySql;
                            }

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message, txt);
                        }
                    }
                }
            }

            if (databaseType == DatabaseType.UnDefined)
            {
                databaseType = DatabaseType.UnKnown;
            }

            for (int i = 0; i < textLines.Count; i++)
            {
                string line = textLines[i];
                string lineLower = line.ToLower();
                if (databaseType == DatabaseType.MsSql)
                {
                    try
                    {
                        if (lineLower.IndexOf("primary key clustered") > -1)
                        {
                            string linePrimaryKey = RemoveBrackets(textLines[i + 2]);
                            var regex = new Regex("asc", RegexOptions.IgnoreCase);
                            linePrimaryKey = regex.Replace(linePrimaryKey, "");
                            regex = new Regex("desc", RegexOptions.IgnoreCase);
                            primaryKey = regex.Replace(linePrimaryKey, "").ToStr().Trim();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message, line);

                    }

                }
                else if (databaseType == DatabaseType.MySql)
                {
                    try
                    {
                        if (lineLower.IndexOf("primary key") > -1)
                        {
                            Regex r = r = new Regex(paranthesesRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            var matches3 = r.Matches(line);
                            primaryKey = RemoveParatheses(RemoveNail(matches3[0].Groups[0].ToStr()));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message, line);

                    }
                }
                else if (databaseType == DatabaseType.MySql)
                {

                }

            }


            for (int i = 0; i < textLines.Count; i++)
            {
                string line = textLines[i];
                string lineLower = line.ToLower();
                var p = new TableRowMetaData();
                var isSqlColumnLine = IsLineColumnFieldLine(line);
                try
                {
                    // Do something with the line
                    var lineRemovedBracket = RemoveBrackets(line);
                    if (lineRemovedBracket.ToLower().StartsWith("use"))
                    {
                        var regex = new Regex("use", RegexOptions.IgnoreCase);
                        databaseName = regex.Replace(lineRemovedBracket, "");
                    }
                    else if (line.ToLower().Contains("create table"))
                    {

                        try
                        {
                            string tbl = "table";
                            var f = lineLower.IndexOf(tbl) + tbl.Length;
                            string s1 = RemoveBrackets(line.Substring(f, line.Length - f - 1).ToStr());
                            tableName = RemoveNail(s1);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message, line);
                        }



                    }
                    else if (isSqlColumnLine)
                    {

                        try
                        {
                            line=line.TrimEnd(',').ToStr();
                            var lineParts = Regex.Split(line, @"\s+").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                            var isNotNull = lineLower.Contains("not null");
                            p.IsNull = isNotNull ? "NO" : "YES";
                            string tbl = isNotNull ? "not null" : "null";


                            if (databaseType == DatabaseType.MsSql)
                            {
                                try
                                {
                                    Regex r = new Regex(bracketsRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                    var matches = r.Matches(line);
                                    if (line.IndexOf("(") > -1 && line.IndexOf(")") > -1)
                                    {
                                        r = new Regex(paranthesesRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                        var matches3 = r.Matches(line);
                                        p.CharacterMaximumLength = matches3[0].Groups[0].ToInt();
                                    }

                                    p.ColumnName = RemoveBrackets(matches[0].Groups[0].ToStr());

                                    p.PrimaryKey = false;
                                    p.ID = counter++;
                                    p.Order = counter++;
                                    p.DataType = RemoveBrackets(matches[1].Groups[0].ToStr());
                                    p.DataTypeMaxChar = p.DataType + p.CharacterMaximumLength.ToStr();
                                    p.DatabaseType = DatabaseType.MsSql;

                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, ex.Message, line);
                                }

                            }
                            else if (databaseType == DatabaseType.MySql)
                            {
                                try
                                {
                                    //  `youtube` varchar(255) DEFAULT NULL,
                                    Regex r = new Regex(mysqlQuoteRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                    var matches = r.Matches(line);
                                    p.ColumnName = RemoveNail(matches[0].Groups[0].ToStr());
                                    p.DataType = lineParts[1];

                                    if (p.DataType.IndexOf("(") > -1 && p.DataType.IndexOf(")") > -1)
                                    {
                                        r = new Regex(paranthesesRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                        var matches3 = r.Matches(line);
                                        p.CharacterMaximumLength = matches3[0].Groups[0].ToInt();
                                        p.DataType = p.DataType.Replace("(" + p.CharacterMaximumLength + ")", "");
                                    }

                                    p.DataTypeMaxChar = lineParts[1];
                                    p.PrimaryKey = false;
                                    p.ID = counter++;
                                    p.Order = counter++;
                                    p.DatabaseType = DatabaseType.MySql;

                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, ex.Message, line);
                                }
                            }
                            else if (databaseType == DatabaseType.UnKnown)
                            {
                                p.ColumnName = lineParts[0];
                                p.DataType = lineParts[1];
                                p.DataTypeMaxChar = lineParts[1];
                                p.PrimaryKey = false;
                                p.ID = counter++;
                                p.Order = counter++;
                                p.DatabaseType = DatabaseType.MySql;
                            }

                            tableMetaDataList.Add(p);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message, line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message, line);

                }

            }
            Console.WriteLine("databaseName:" + databaseName);
            Console.WriteLine("tableName:" + tableName);
            Console.WriteLine("primaryKey:" + primaryKey);
            string tableSchema = "";
            if (tableName.Contains("."))
            {
                tableSchema = tableName.Split(".".ToCharArray()).FirstOrDefault().ToStr();
            }
            databaseMetaData.SelectedTable.TableName = RemoveNail(tableName);
            databaseMetaData.SelectedTable.TableCatalog = databaseName;
            databaseMetaData.SelectedTable.TableType = "";
            databaseMetaData.SelectedTable.TableSchema = tableSchema;

            databaseMetaData.DatabaseType = databaseType;
            return databaseMetaData;
        }




        private  List<String> MySqlDataTypeList
        {
            get
            {

                var sqlDataType = new List<String>();
                sqlDataType.Add("Decimal");
                sqlDataType.Add("Byte");
                sqlDataType.Add("Int16");
                sqlDataType.Add("Int32");
                sqlDataType.Add("Float");
                sqlDataType.Add("Double");
                sqlDataType.Add("Timestamp");
                sqlDataType.Add("Int64");
                sqlDataType.Add("Int24");
                sqlDataType.Add("Date");
                sqlDataType.Add("Time");
                sqlDataType.Add("DateTime");
                sqlDataType.Add("Datetime");
                sqlDataType.Add("Year");
                sqlDataType.Add("Newdate");
                sqlDataType.Add("VarString");
                sqlDataType.Add("Bit");
                sqlDataType.Add("JSON");
                sqlDataType.Add("NewDecimal");
                sqlDataType.Add("Enum");
                sqlDataType.Add("Set");
                sqlDataType.Add("TinyBlob");
                sqlDataType.Add("MediumBlob");
                sqlDataType.Add("LongBlob");
                sqlDataType.Add("Blob");
                sqlDataType.Add("VarChar");
                sqlDataType.Add("String");
                sqlDataType.Add("Geometry");
                sqlDataType.Add("UByte");
                sqlDataType.Add("UInt16");
                sqlDataType.Add("UInt32");
                sqlDataType.Add("UInt64");
                sqlDataType.Add("UInt24");
                sqlDataType.Add("Binary");
                sqlDataType.Add("VarBinary");
                sqlDataType.Add("TinyText");
                sqlDataType.Add("MediumText");
                sqlDataType.Add("LongText");
                sqlDataType.Add("Text");
                sqlDataType.Add("Guid");

                return sqlDataType;

            }
        }
        private  bool IsLineColumnFieldLine(string line)
        {
            return IsMySqlDataType(line) || IsSqlDataType(line);
        }
        private  bool IsMySqlDataType(string line)
        {
            var sqlDataType = MySqlDataTypeList;
            foreach (var item in sqlDataType)
            {
                if (line.IndexOf(item) > -1)
                {
                    return true;
                }
            }

            return false;
        }
        private  List<String> SqlServerDataTypes
        {
            get
            {
                var sqlDataType = new List<String>();
                sqlDataType.Add("varbinary");
                sqlDataType.Add("binary");
                sqlDataType.Add("image");
                sqlDataType.Add("varchar");
                sqlDataType.Add("char");
                sqlDataType.Add("nvarchar");
                sqlDataType.Add("nchar");
                sqlDataType.Add("text");
                sqlDataType.Add("ntext");
                sqlDataType.Add("uniqueidentifier");
                sqlDataType.Add("rowversion");
                sqlDataType.Add("bit");
                sqlDataType.Add("tinyint");
                sqlDataType.Add("smallint");
                sqlDataType.Add("int");
                sqlDataType.Add("bigint");
                sqlDataType.Add("smallmoney");
                sqlDataType.Add("money");
                sqlDataType.Add("numeric");
                sqlDataType.Add("decimal");
                sqlDataType.Add("real");
                sqlDataType.Add("float");
                sqlDataType.Add("smalldatetime");
                sqlDataType.Add("datetime");
                sqlDataType.Add("sql_variant");
                sqlDataType.Add("table");
                sqlDataType.Add("cursor");
                sqlDataType.Add("timestamp");
                sqlDataType.Add("xml");

                return sqlDataType;
            }
        }
        private  bool IsSqlDataType(string line)
        {
            var sqlDataType = SqlServerDataTypes;
            foreach (var item in sqlDataType)
            {
                if (line.IndexOf(item) > -1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text.RegularExpressions;
using DotNetCodeGenerator.Domain.Entities;
using DotNetCodeGenerator.Domain.Helpers;
using MySql.Data.MySqlClient;
using DotNetCodeGenerator.Domain.Entities.Enums;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DotNetCodeGenerator.Domain.Repositories
{
    public class TableRepository : ITableRepository
    {


        private readonly ILogger Logger;


        public TableRepository(ILogger<TableRepository> logger)
        {
            Logger = logger;
        }


        public void GetSelectedMysqlTableMetaData(DatabaseMetadata databaseMetaData, string selectedTable)
        {

            //            To get column information with SqlClient or other providers you do: 
            //DataTable schema = conn.GetSchema("Columns", new string[4] { conn.Database, null, "products", null });

            //            With MySQL Connector / NET is different: 
            //DataTable schema = conn.GetSchema("Columns", new string[4] { null, conn.Database, "products", null });

            //            Note that the first 2 items in the array are swapped. 
            var selectedTableWithDatabase = selectedTable.Split("-".ToCharArray()).FirstOrDefault().ToStr();
            var con = new MySqlConnection(databaseMetaData.MySqlConnectionString);
            con.Open();
            string[] objArrRestrict;
            var tParts = selectedTableWithDatabase.Split(".".ToCharArray());
            objArrRestrict = new string[] {null,
                con.Database,
                tParts[2],
                null
                 };
            DataTable tbl = con.GetSchema("Columns", objArrRestrict);


            List<TableRowMetaData> TableRowMetaDataList = new List<TableRowMetaData>();
            var selectedTableObj = databaseMetaData.Tables.FirstOrDefault(r => r.DatabaseTableName.Equals(selectedTableWithDatabase, StringComparison.InvariantCultureIgnoreCase));

            if (selectedTableObj != null)
            {
                selectedTableObj.TableRowMetaDataList = TableRowMetaDataList;
                databaseMetaData.SelectedTable = selectedTableObj;
                int i = 0;
                foreach (DataRow rowTable in tbl.Rows)
                {
                    try
                    {


                        String TABLE_CATALOG = rowTable["TABLE_CATALOG"].ToStr();
                        String TABLE_SCHEMA = rowTable["TABLE_SCHEMA"].ToStr();
                        String TABLE_NAME = rowTable["TABLE_NAME"].ToStr();
                        String COLUMN_NAME = rowTable["COLUMN_NAME"].ToStr();
                        String ORDINAL_POSITION = rowTable["ORDINAL_POSITION"].ToStr();
                        String COLUMN_DEFAULT = rowTable["COLUMN_DEFAULT"].ToStr();
                        String IS_NULLABLE = rowTable["IS_NULLABLE"].ToStr();
                        String DATA_TYPE = rowTable["DATA_TYPE"].ToStr();
                        var CHARACTER_MAXIMUM_LENGTH = rowTable["CHARACTER_MAXIMUM_LENGTH"].ToInt();
                        var NUMERIC_PRECISION = DataTableHelper.GetValueInt(rowTable, "NUMERIC_PRECISION", 0);
                        var NUMERIC_SCALE = DataTableHelper.GetValueInt(rowTable, "NUMERIC_SCALE", 0);
                        //String CHARACTER_SET_NAME = rowTable["CHARACTER_SET_NAME"].ToStr();
                        //String COLLATION_NAME = rowTable["COLLATION_NAME"].ToStr();
                        //String COLUMN_TYPE = rowTable["COLUMN_TYPE"].ToStr();
                        String COLUMN_KEY = DataTableHelper.GetValueString(rowTable, "COLUMN_KEY","");
                        //String EXTRA = rowTable["EXTRA"].ToStr();
                        //String PRIVILEGES = rowTable["PRIVILEGES"].ToStr();
                        //String COLUMN_COMMENT = rowTable["COLUMN_COMMENT"].ToStr();


                        var k = new TableRowMetaData();
                        k.DatabaseType = DatabaseType.MySql;
                        k.ID = i++;
                        k.ColumnName = COLUMN_NAME;
                        k.DataType = DATA_TYPE;
                        k.IsNull = IS_NULLABLE;
                        k.CharacterMaximumLength = CHARACTER_MAXIMUM_LENGTH;
                        k.DataTypeMaxChar = k.DataType;
                        k.NumericPrecision = NUMERIC_PRECISION;
                        k.NumericScale = NUMERIC_SCALE;
                        if (k.DataType.Contains("varchar"))
                        {
                            k.CharacterMaximumLength = CHARACTER_MAXIMUM_LENGTH;
                            k.DataTypeMaxChar = k.DataType + "(" + k.CharacterMaximumLength + ")";
                        }
                        k.Order = ORDINAL_POSITION.ToInt();
                        k.ID = ++i;
                        k.PrimaryKey = COLUMN_KEY.Equals("PRI", StringComparison.InvariantCultureIgnoreCase);

                        TableRowMetaDataList.Add(k);

                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.Message);
                    }
                }

            }
            con.Close();



        }

        public DatabaseMetadata GetAllMySqlTables(String connectionString)
        {
            var result = new DatabaseMetadata();
            var con = new MySqlConnection(connectionString);
            result.DatabaseType = DatabaseType.MySql;

            try
            {

                con.Open();



                result.DatabaseName = con.Database;
                DataTable tblDatabases =
                                con.GetSchema("Tables");

                var list = new List<TableMetaData>();

                foreach (DataRow rowTable in tblDatabases.Rows)
                {
                    var i = new TableMetaData();
                    String TABLE_CATALOG = rowTable["TABLE_CATALOG"].ToStr();
                    String TABLE_SCHEMA = rowTable["TABLE_SCHEMA"].ToStr();
                    String TABLE_NAME = rowTable["TABLE_NAME"].ToStr();
                    String TABLE_TYPE = rowTable["TABLE_TYPE"].ToStr();
                    String ENGINE = rowTable["ENGINE"].ToStr();
                    String VERSION = rowTable["VERSION"].ToStr();
                    String ROW_FORMAT = rowTable["ROW_FORMAT"].ToStr();
                    String TABLE_ROWS = rowTable["TABLE_ROWS"].ToStr();
                    String AVG_ROW_LENGTH = rowTable["AVG_ROW_LENGTH"].ToStr();
                    String DATA_LENGTH = rowTable["DATA_LENGTH"].ToStr();
                    String MAX_DATA_LENGTH = rowTable["MAX_DATA_LENGTH"].ToStr();
                    String INDEX_LENGTH = rowTable["INDEX_LENGTH"].ToStr();
                    String DATA_FREE = rowTable["DATA_FREE"].ToStr();
                    String AUTO_INCREMENT = rowTable["AUTO_INCREMENT"].ToStr();
                    String CREATE_TIME = rowTable["CREATE_TIME"].ToStr();
                    String UPDATE_TIME = rowTable["UPDATE_TIME"].ToStr();
                    String CHECK_TIME = rowTable["CHECK_TIME"].ToStr();
                    String TABLE_COLLATION = rowTable["TABLE_COLLATION"].ToStr();
                    String CHECKSUM = rowTable["CHECKSUM"].ToStr();
                    String CREATE_OPTIONS = rowTable["CREATE_OPTIONS"].ToStr();
                    String TABLE_COMMENT = rowTable["TABLE_COMMENT"].ToStr();

                    i.TableCatalog = TABLE_CATALOG;
                    i.TableSchema = TABLE_SCHEMA;
                    i.TableName = TABLE_NAME;
                    i.TableType = TABLE_TYPE;
                    list.Add(i);
                }

                con.Close();

                result.MySqlConnectionString = connectionString;
                result.Tables = list.OrderBy(t => t.TableName).ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                throw e;
            }

            return result;
        }

        public DatabaseMetadata GetAllTables(String connectionString)
        {
            var result = new DatabaseMetadata();
            SqlConnection con =
                          new SqlConnection(connectionString);
            result.DatabaseType = DatabaseType.MsSql;

            try
            {

                con.Open();



                result.DatabaseName = con.Database;
                DataTable tblDatabases =
                                con.GetSchema(
                                           SqlClientMetaDataCollectionNames.Tables);


                var list = new List<TableMetaData>();
                foreach (DataRow rowDatabase in tblDatabases.Rows)
                {
                    var i = new TableMetaData();
                    i.TableCatalog = rowDatabase["table_catalog"].ToStr();
                    i.TableSchema = rowDatabase["table_schema"].ToStr();
                    i.TableName = rowDatabase["table_name"].ToStr();
                    i.TableType = rowDatabase["table_type"].ToStr();
                    list.Add(i);
                }

                con.Close();

                result.ConnectionString = connectionString;
                result.Tables = list.OrderBy(t => t.TableName).ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                throw e;
            }

            return result;
        }
        public void GetSelectedTableMetaData(DatabaseMetadata databaseMetaData, string selectedTable)
        {

            try
            {
                var selectedTableWithDatabase = selectedTable.Split("-".ToCharArray()).FirstOrDefault().ToStr();
                var builder = new SqlConnectionStringBuilder(databaseMetaData.ConnectionString);
                var con = new SqlConnection(builder.ConnectionString);
                con.Open();

                string[] objArrRestrict;
                var tParts = selectedTableWithDatabase.Split(".".ToCharArray());
                objArrRestrict = new string[] {
                tParts[0],
                tParts[1],
                tParts[2],
                null };
                DataTable tbl = con.GetSchema(SqlClientMetaDataCollectionNames.Columns, objArrRestrict);

                SqlDataAdapter da = new SqlDataAdapter();
                String primaryKey = "";
                try
                {
                    #region Get Primary Key
                    DataTable ttt = new DataTable();
                    SqlCommand cmd = new SqlCommand("select * from " + selectedTableWithDatabase);
                    cmd.Connection = con;
                    SqlDataAdapter daa = new SqlDataAdapter();
                    daa.SelectCommand = cmd;
                    //da.Fill(tl);
                    daa.FillSchema(ttt, SchemaType.Mapped);
                    primaryKey = DataTableHelper.GetPrimaryKeys(ttt);

                    #endregion

                }
                catch (Exception e)
                {
                    Logger.LogError(e, e.Message);
                    throw e;
                }




                List<TableRowMetaData> TableRowMetaDataList = new List<TableRowMetaData>();
                var selectedTableObj = databaseMetaData.Tables.FirstOrDefault(r => r.DatabaseTableName.Equals(selectedTableWithDatabase, StringComparison.InvariantCultureIgnoreCase));

                if (selectedTableObj != null)
                {
                    selectedTableObj.TableRowMetaDataList = TableRowMetaDataList;
                    databaseMetaData.SelectedTable = selectedTableObj;
                    int i = 0;

                    foreach (DataRow rowTable in tbl.Rows)
                    {
                        try
                        {
                            String TABLE_CATALOG = rowTable["TABLE_CATALOG"].ToStr();
                            String TABLE_SCHEMA = rowTable["TABLE_SCHEMA"].ToStr();
                            String TABLE_NAME = rowTable["TABLE_NAME"].ToStr();
                            String COLUMN_NAME = rowTable["COLUMN_NAME"].ToStr();
                            String ORDINAL_POSITION = rowTable["ORDINAL_POSITION"].ToStr();
                            String COLUMN_DEFAULT = rowTable["COLUMN_DEFAULT"].ToStr();
                            String IS_NULLABLE = rowTable["IS_NULLABLE"].ToStr();
                            String DATA_TYPE = rowTable["DATA_TYPE"].ToStr();
                            var CHARACTER_MAXIMUM_LENGTH = rowTable["CHARACTER_MAXIMUM_LENGTH"].ToInt();
                            String CHARACTER_OCTET_LENGTH = rowTable["CHARACTER_OCTET_LENGTH"].ToStr();
                            var NUMERIC_PRECISION = rowTable["NUMERIC_PRECISION"].ToInt();
                            var NUMERIC_PRECISION_RADIX = rowTable["NUMERIC_PRECISION_RADIX"].ToStr();
                            var NUMERIC_SCALE = rowTable["NUMERIC_SCALE"].ToInt();
                            String DATETIME_PRECISION = rowTable["DATETIME_PRECISION"].ToStr();
                            String CHARACTER_SET_CATALOG = rowTable["CHARACTER_SET_CATALOG"].ToStr();
                            String CHARACTER_SET_SCHEMA = rowTable["CHARACTER_SET_SCHEMA"].ToStr();
                            String CHARACTER_SET_NAME = rowTable["CHARACTER_SET_NAME"].ToStr();
                            String COLLATION_CATALOG = rowTable["COLLATION_CATALOG"].ToStr();
                            String IS_SPARSE = rowTable["IS_SPARSE"].ToStr();
                            String IS_COLUMN_SET = rowTable["IS_COLUMN_SET"].ToStr();
                            String IS_FILESTREAM = rowTable["IS_FILESTREAM"].ToStr();


                            var k = new TableRowMetaData();
                            k.DatabaseType = DatabaseType.MsSql;
                            k.ColumnName = COLUMN_NAME;
                            k.DataType = DATA_TYPE;
                            k.IsNull = IS_NULLABLE;
                            k.CharacterMaximumLength = CHARACTER_MAXIMUM_LENGTH;
                           
                            if (k.DataType.Contains("varchar"))
                            {
                                k.CharacterMaximumLength = CHARACTER_MAXIMUM_LENGTH;
                              //  k.DataTypeMaxChar = k.DataType + "(" + k.CharacterMaximumLength + ")";
                            }

                            k.DataTypeMaxChar = k.TypeDeclaration();
                            k.Order = ORDINAL_POSITION.ToInt();
                            k.ID = ++i;
                            k.PrimaryKey = COLUMN_NAME == primaryKey;
                            TableRowMetaDataList.Add(k);

                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                     
                    }

                }
                con.Close();

            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                throw e;
            }

        }
        public DataSet GetDataSet(string sqlCommand, string connectionString)
        {
            DataSet ds = new DataSet();
            try
            {
                if (!String.IsNullOrEmpty(sqlCommand))
                {


                    var queryParts = Regex.Split(sqlCommand, @"\s+").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                    String sp = queryParts.FirstOrDefault();
                    sqlCommand = sqlCommand.Replace(sp, "");


                    SqlConnection conn = new SqlConnection(connectionString);
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sp;
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (!String.IsNullOrEmpty(sqlCommand))
                    {
                        var queryParts2 = Regex.Split(sqlCommand, @",").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                        foreach (var item in queryParts2)
                        {
                            var parameterParts = Regex.Split(item, @"=").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                            var paraterValue = parameterParts.LastOrDefault().Replace("'", "");
                            var paramterName = parameterParts.FirstOrDefault();
                            if (paraterValue.ToLower().Equals("null", StringComparison.InvariantCultureIgnoreCase))
                            {
                                cmd.Parameters.Add(new SqlParameter(paramterName, DBNull.Value));
                            }
                            else
                            {
                                cmd.Parameters.Add(new SqlParameter(paramterName, paraterValue));
                            }

                        }
                    }

                    da.SelectCommand = cmd;


                    conn.Open();
                    da.Fill(ds);
                    conn.Close();

                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                throw e;
            }
            return ds;
        }
    }



}

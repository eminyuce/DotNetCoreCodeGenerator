using DotNetCodeGenerator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCodeGenerator.Domain.Helpers
{
    public class TableRowMetaDataHelper
    {
        public static string GetSqlDataTypeFromColumnDataType(TableRowMetaData ki)
        {

            String result = "SqlDbType.{0}";
            var item = ki;
            if (item.DataType.IndexOf("varchar") > -1 || item.DataType.IndexOf("text") > -1)
            {
                result = String.Format(result, "NVarChar");
            }
            else if (item.DataType.IndexOf("int") > -1)
            {
                result = String.Format(result, "Int");
            }
            else if (item.DataType.IndexOf("date") > -1)
            {
                result = String.Format(result, "DateTime");
            }
            else if (item.DataType.IndexOf("bit") > -1)
            {
                result = String.Format(result, "Bit");
            }
            else if (item.DataType.IndexOf("float") > -1)
            {
                result = String.Format(result, "Float");
            }
            else if (item.DataType.IndexOf("char") > -1)
            {
                result = String.Format(result, "NVarChar");
            }
            else if (item.DataType.IndexOf("xml") > -1)
            {
                result = String.Format(result, "Xml");
            }
            else
            {
                result = GeneralHelper.ConvertTypeToSQL(item.DataType);

            }


            return result;
        }
        public static string GetCSharpDataType(TableRowMetaData ki)
        {
            var item = ki;
            String result = "";
            if (item.DataType.IndexOf("varchar") > -1 || item.DataType.IndexOf("text") > -1 || item.DataType.IndexOf("xml") > -1)
            {
                result = "String";
            }
            else if (item.DataType.IndexOf("int") > -1)
            {
                result = "int";
            }
            else if (item.DataType.IndexOf("date") > -1)
            {
                result = "DateTime ";
            }
            else if (item.DataType.IndexOf("bit") > -1)
            {
                result = "Boolean ";
            }
            else if (item.DataType.IndexOf("float") > -1)
            {
                result = "float ";
            }
            else if (item.DataType.IndexOf("char") > -1)
            {
                result = "char ";
            }

            return result.Trim();
        }
       
        public static string GetPrimaryKeys(List<TableRowMetaData> tableRowMetaDataList)
        {
            var firstOrDefault = GetPrimaryKeysObj(tableRowMetaDataList);
            if (firstOrDefault != null)
                return firstOrDefault.ColumnName;
            else
                return "";
        }
        public static TableRowMetaData GetPrimaryKeysObj(List<TableRowMetaData> tableRowMetaDataList)
        {
            foreach (var item in tableRowMetaDataList)
            {
                if (item.PrimaryKey)
                {
                    return item;
                }
            }
            var firstOrDefault = tableRowMetaDataList.FirstOrDefault();
            if (firstOrDefault != null)
                return firstOrDefault;
            else
                return null;
        }
    }
}

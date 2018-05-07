using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCodeGenerator.Domain.Helpers
{
    public class DataTableHelper
    {
        public static object GetValue(DataRow row, string column)
        {
            return row.Table.Columns.Contains(column) ? row[column] : null;
        }
        public static string GetPrimaryKeys(DataTable myTable)
        {
            // Create the array for the columns.
            DataColumn[] colArr;
            colArr = myTable.PrimaryKey;
            // Get the number of elements in the array.
            //Response.Write("Column Count: " + colArr.Length + "<br>");
            string primaryKey = "";
            for (int i = 0; i < colArr.Length; i++)
            {
                //Response.Write(colArr[i].ColumnName + "<br>" + colArr[i].DataType + "<br>");
                primaryKey = colArr[i].ColumnName;
            }
            return primaryKey;
        }

        public static String ToPrintConsole(DataTable dataTable)
        {
            var sb = new StringBuilder();
            // Print top line
            Console.WriteLine(new string('-', 75));
            sb.AppendLine(new string('-', 1500));
            // Print col headers
            var colHeaders = dataTable.Columns.Cast<DataColumn>().Select(arg => arg.ColumnName);
            foreach (String s in colHeaders)
            {
                Console.Write("| {0,-20}", s);
                sb.Append(String.Format("| {0,-80}", s));
            }
            Console.WriteLine();
            sb.AppendLine();
            // Print line below col headers
            Console.WriteLine(new string('-', 75));
            sb.AppendLine(new string('-', 1500));
            // Print rows
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (Object o in row.ItemArray)
                {
                    Console.Write("| {0,-20}", o.ToString());
                    sb.Append(String.Format("| {0,-80}", o.ToString()));
                }
                Console.WriteLine();
                sb.AppendLine("");
            }

            // Print bottom line
            Console.WriteLine(new string('-', 75));
            sb.AppendLine(new string('-', 1500));

            return sb.ToString();
        }
    }
}

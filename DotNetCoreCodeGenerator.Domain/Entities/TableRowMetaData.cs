using DotNetCodeGenerator.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCodeGenerator.Domain.Entities
{
    [Serializable]
    public class TableRowMetaData
    {
        public DatabaseType DatabaseType { set; get; }
        public String ColumnName { set; get; }
        public String ColumnNameInput { get { return String.Format("p_{0}", ColumnName); }  }
        public String IsNull { set; get; }
        public String DataType { set; get; }
        public String MaxChar { set; get; }
        public String DataTypeMaxChar { set; get; }
        public String CssClass { set; get; }
        public int Order { set; get; }
        public int ID { set; get; }
        public bool PrimaryKey { set; get; }
        public String ControlID { set; get; }
        public bool ForeignKey { get; set; }
        public string ColumnDefaultValue
        {
            get
            {

                String m = "";
                if (DataType.IndexOf("text") > -1)
                {
                    m = "''";
                }
                else if (DataType.IndexOf("varchar") > -1)
                {
                    m = "''";
                }
                else if (DataType.IndexOf("char") > -1)
                {
                    m = "''";
                }
                else if (DataType.IndexOf("int") > -1)
                {
                    m = "0";
                }
                else if (DataType.IndexOf("date") > -1)
                {
                    m = "null";
                }
                else if (DataType.IndexOf("bit") > -1)
                {
                    m = "true";
                }
                else if (DataType.IndexOf("float") > -1)
                {
                    m = "0";
                }

                return m;

            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3} {4}", ColumnName, DataType, DataTypeMaxChar, MaxChar, IsNull);
        }
    }
}

using DotNetCodeGenerator.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCodeGenerator.Domain.Entities
{
    [Serializable]
    public class DatabaseMetadata
    {
        public DatabaseType DatabaseType { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public List<TableMetaData> Tables { get; set; }
        public TableMetaData SelectedTable { get; set; }
        public string MySqlConnectionString { get; set; }
    }
}

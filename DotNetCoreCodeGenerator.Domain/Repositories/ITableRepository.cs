using System.Data;
using DotNetCodeGenerator.Domain.Entities;

namespace DotNetCodeGenerator.Domain.Repositories
{
    public interface ITableRepository
    {
        DatabaseMetadata GetAllMySqlTables(string connectionString);
        DatabaseMetadata GetAllTables(string connectionString);
        DataSet GetDataSet(string sqlCommand, string connectionString);
        void GetSelectedMysqlTableMetaData(DatabaseMetadata databaseMetaData, string selectedTable);
        void GetSelectedTableMetaData(DatabaseMetadata databaseMetaData, string selectedTable);
    }
}
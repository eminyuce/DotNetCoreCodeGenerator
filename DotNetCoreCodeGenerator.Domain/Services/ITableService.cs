using System.Data;
using System.Threading.Tasks;
using DotNetCodeGenerator.Domain.Entities;

namespace DotNetCodeGenerator.Domain.Services
{
    public interface ITableService
    {
        Task FillGridView(CodeGeneratorResult codeGeneratorResult);
        Task GenerateCode(CodeGeneratorResult codeGeneratorResult);
        DatabaseMetadata GetAllMySqlTables(string connectionString);
        DatabaseMetadata GetAllTables(string connectionString);
        DatabaseMetadata GetAllTablesFromCache(string connectionString);
        DatabaseMetadata GetAllMySqlTablesFromCache(string connectionString);
        DataSet GetDataSet(string sqlCommand, string connectionString);
    }
}
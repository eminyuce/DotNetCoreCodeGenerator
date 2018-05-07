using DotNetCodeGenerator.Domain.Entities;

namespace DotNetCodeGenerator.Domain.Helpers
{
    public interface ISqlParserHelper
    {
        DatabaseMetadata ParseSqlCreateStatement(string txt = "");
    }
}
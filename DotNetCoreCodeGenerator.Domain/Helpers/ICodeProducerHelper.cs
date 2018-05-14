using DotNetCodeGenerator.Domain.Entities;

namespace DotNetCodeGenerator.Domain.Helpers
{
    public interface ICodeProducerHelper
    {

        CodeGeneratorResult CodeGeneratorResult { get; set; }
        DatabaseMetadata DatabaseMetadata { get; set; }

        void GenerateAspMvcControllerClass();
        void GenerateMySqlSaveOrUpdateStoredProcedure();
        void GenerateNewInstance();
        void GenerateSaveOrUpdateStoredProcedure();
        void GenerateMergeSqlStoredProcedure();
        void GenerateSqlRepository();
        void GenerateStoredProcExecutionCode();
        void GenerateTableItem();
        void GenerateTableServices();
        void GenerateWebApiController();
        void GenereateMySqlRepository();
    }
}

namespace project.DataBase.CreateTableFromQuery.Abstraction
{
    public interface ITableCreator
    {
        Task CreateTableFromQueryAsync(string sourceQuery,string sourceConnectionString, string destinationConnectionString, string newTableName);
    }
}


namespace project.CreateTableFromQuery.Abstraction
{
    public interface ITableCreator
    {
        Task CreateTableFromQueryAsync(string sourceConnectionString, string sourceQuery, string destinationConnectionString, string newTableName);
    }
}

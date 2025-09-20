
namespace project.DataBase.CreateTableFromQuery.Abstraction
{
    public interface ITransferTable
    {
        Task Transfer(string sourceQuery,string sourceConnectionString, string destinationConnectionString, string newTableName,CancellationToken cancellationToken);
    }
}

namespace project.TransferTablefromQuery.Abstraction
{
    public interface IDataInserter
    {
        Task TransferDataAsync(string sourceConnectionString, string sourceQuery, string destinationConnectionString, string newTableName,CancellationToken cancellationToken);
    }
}
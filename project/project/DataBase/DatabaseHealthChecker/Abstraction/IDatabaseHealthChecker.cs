namespace project.DatabaseHealthChecker.Abstraction;

public interface IDatabaseHealthChecker
{
    Task<bool> IsConnectionValidAsync(string connectionString,CancellationToken cancellationToken);
    Task<bool> TableHasDataAsync(string connectionString, string tableName,CancellationToken cancellationToken);
}

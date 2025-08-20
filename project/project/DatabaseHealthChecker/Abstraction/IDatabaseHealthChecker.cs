namespace project.DatabaseHealthChecker.Abstraction;

public interface IDatabaseHealthChecker
{
    Task<bool> IsConnectionValidAsync(string connectionString);
    Task<bool> TableHasDataAsync(string connectionString, string tableName);
}

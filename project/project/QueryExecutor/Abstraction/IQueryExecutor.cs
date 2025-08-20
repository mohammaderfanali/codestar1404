using System.Data;

namespace project.QueryExecutor.Abstraction;

public interface IQueryExecutor
{
    public Task<DataTable> ExecuteQueryAsync(KeyValuePair<string, string> queryPair);

}
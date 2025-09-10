using System.Data;

namespace project.DataBase.QueryExecutor.Abstraction
{
    public interface ISelectQueryExecutor
    {
        Task<DataTable> ExecuteQueryAsync(string query, string connectionString);
    }
}
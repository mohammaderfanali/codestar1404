using System.Data;
using System.Threading.Tasks;

namespace project.QueryExecutor.Abstraction
{
    public interface IQueryExecutor
    {
        Task<DataTable> ExecuteQueryAsync(string query, string connectionString);
    }
}
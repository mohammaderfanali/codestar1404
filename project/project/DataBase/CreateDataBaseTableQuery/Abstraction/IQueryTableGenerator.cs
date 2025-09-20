using System.Data;

public interface IQueryTableGenerator
{
    string GenerateCreateTableSql(string tableName, DataColumnCollection columns);
}
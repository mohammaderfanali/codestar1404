using System.Data;

namespace project.Helpers.ColumnNameMaker.Abstraction;

public interface IColumnNameResolver
{
    List<string> ResolveUniqueNames(DataColumnCollection columns);
}
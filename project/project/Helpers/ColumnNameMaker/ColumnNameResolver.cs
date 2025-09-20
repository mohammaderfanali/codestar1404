using System.Data;
using project.Helpers.ColumnNameMaker.Abstraction;

namespace project.Helpers.ColumnNameMaker;

public class ColumnNameResolver : IColumnNameResolver
{
    public List<string> ResolveUniqueNames(DataColumnCollection columns)
    {
        var processed = new HashSet<string>();
        var result = new List<string>();

        foreach (DataColumn column in columns)
        {
            var name = column.ColumnName;
            var finalName = name;
            int suffix = 1;

            while (processed.Contains(finalName))
                finalName = $"{name}{suffix++}";

            processed.Add(finalName);
            result.Add(finalName);
        }

        return result;
    }
}
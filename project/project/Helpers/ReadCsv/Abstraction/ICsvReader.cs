using System.Data;

namespace project.ReadCsv.Abstraction
{
    public interface ICsvReader
    {
        DataTable ReadCsvFile(string filePath);
    }
}
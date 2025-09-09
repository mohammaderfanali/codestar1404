namespace project.csvReader.Abstraction;

public interface ICsvReader
{
    List<string[]> ReadCsvFile(string filePath);
    string[] GetColumnHeaders(string filePath);
    public string GetFileName(string filePath);
}

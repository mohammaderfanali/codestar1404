namespace project.csvReader.Abstraction;

public interface ICsvReader
{
    List<string[]> ReadCsvFile(string filePath);
}
using project.csvReader.Abstraction;

namespace project.csvReader;
using Microsoft.Extensions.Logging;

public class CsvReader : ICsvReader
{
    private readonly ILogger<CsvReader> _logger; 
    public CsvReader(ILogger<CsvReader> logger)
    {
        _logger = logger;
    }

    public List<string[]> ReadCsvFile(string filePath)
    {
        var data = new List<string[]>();
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines.Skip(1))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    string[] values = line.Split(',');
                    data.Add(values);
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "The file at path {FilePath} was not found.", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while reading the file {FilePath}.", filePath);
        }

        return data;
    }
    
    public string GetFileName(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found at the specified path.", filePath);
            }
            return Path.GetFileName(filePath);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "GetFileName failed because the file at {FilePath} was not found.", filePath);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred in GetFileName for path {FilePath}.", filePath);
            throw;
        }
    }


public string[] GetColumnHeaders(string filePath)
{
    try
    {

        var headerLine = File.ReadLines(filePath).FirstOrDefault();

        if (string.IsNullOrEmpty(headerLine))
        {
            _logger.LogWarning("CSV file at {FilePath} is empty or has an empty header.", filePath);
            return Array.Empty<string>();
        }

        return headerLine.Split(',');
    }
    catch (FileNotFoundException ex)
    {
        _logger.LogError(ex, "The file at path {FilePath} was not found.", filePath);
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An unexpected error occurred while reading the header from {FilePath}.", filePath);
        throw;
    }
}
}
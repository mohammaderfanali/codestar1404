using Microsoft.Extensions.Logging;
using project.ReadCsv.Abstraction;

namespace project.ReadCsv;

public class CsvReader(ILogger<CsvReader> logger) : ICsvReader
{
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
            logger.LogError(ex, "The file at path {FilePath} was not found.", filePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while reading the file {FilePath}.", filePath);
        }


        return data;
    }

    public string GetFileName(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                logger.LogWarning("The file at {FilePath} was not found.", filePath);
            }

            return Path.GetFileNameWithoutExtension(filePath);
        }
        catch (FileNotFoundException ex)
        {
            logger.LogError(ex, "GetFileName failed because the file at {FilePath} was not found.", filePath);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in GetFileName for path {FilePath}.", filePath);
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
                logger.LogWarning("CSV file at {FilePath} is empty or has an empty header.", filePath);
                return Array.Empty<string>();
            }

            return headerLine.Split(',');
        }
        catch (FileNotFoundException ex)
        {
            logger.LogError(ex, "The file at path {FilePath} was not found.", filePath);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while reading the header from {FilePath}.", filePath);
            throw;
        }
    }
}
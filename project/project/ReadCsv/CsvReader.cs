using System;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using project.ReadCsv.Abstraction;

namespace project.ReadCsv
{
    public class CsvReader : ICsvReader
    {
        private readonly ILogger<CsvReader> _logger;

        public CsvReader(ILogger<CsvReader> logger)
        {
            _logger = logger;
        }

        public DataTable ReadCsvFile(string filePath)
        {
            var dataTable = new DataTable();
            try
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length == 0)
                {
                    _logger.LogWarning("CSV file at {FilePath} is empty.", filePath);
                    return dataTable;
                }

                dataTable.TableName = Path.GetFileNameWithoutExtension(filePath);
                var headers = lines[0].Split(',');
                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header.Trim());
                }

                foreach (string line in lines.Skip(1))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var values = line.Split(',');
                        if (values.Length != dataTable.Columns.Count)
                        {
                            _logger.LogWarning(
                                "Row has {ValueCount} values but expected {ColumnCount}. Skipping line: {Line}",
                                values.Length, dataTable.Columns.Count, line);
                            continue;
                        }

                        dataTable.Rows.Add(values);
                    }
                }

                _logger.LogInformation("Successfully read {RowCount} data rows from {FilePath}.", dataTable.Rows.Count,
                    filePath);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "The CSV file at path {FilePath} was not found.", filePath);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while reading the CSV file at {FilePath}.",
                    filePath);
                throw;
            }

            return dataTable;
        }
    }
}
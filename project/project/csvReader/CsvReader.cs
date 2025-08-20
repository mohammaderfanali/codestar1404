namespace project.csvReader;

public class CsvReader
{
    public List<string[]> ReadCsvFile(string filePath)
    {
        var data = new List<string[]>();
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    string[] values = line.Split(',');
                    data.Add(values);
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: The file at path '{filePath}' was not found.");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            return null;
        }
        return data;
    }
    
    
    
    public string GetFileName(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: File not found at '{filePath}'.");
            return null;
        }

        return Path.GetFileName(filePath);
    }


}
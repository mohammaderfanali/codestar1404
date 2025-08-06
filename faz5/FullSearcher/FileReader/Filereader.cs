namespace SearchEngine;

public class FileReader:IFileReader
{
    
    public Dictionary<string, string> ReadFiles(string path)
    {
        return Directory.GetFiles(path)
            .ToDictionary(Path.GetFileName, File.ReadAllText);
    }
}
namespace file;

public interface IFileReader
{
    Dictionary<string, string> ReadFiles(string path);
}
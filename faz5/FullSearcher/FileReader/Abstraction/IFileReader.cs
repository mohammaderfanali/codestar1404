namespace  SearchEngine;

public interface IFileReader
{
    Dictionary<string, string> ReadFiles(string path);
}
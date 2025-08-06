namespace SearchEngine;

public interface ISearcher
{
    List<string> Search(string query);
}
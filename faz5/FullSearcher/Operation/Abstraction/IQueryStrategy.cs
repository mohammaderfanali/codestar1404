namespace SearchEngine;

public interface IQueryStrategy
{
    List<string> Execute(string query);
}
namespace SearchEngine;

public interface IQueryHandler
{
    public List<(string value, char prefix)> SplitCommandWithRegex(string input);
}
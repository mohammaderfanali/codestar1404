namespace SearchEngine;

public interface ITokenizer
{
    List<string> Tokenize(string content);
}
namespace faz3;

public interface IWorkOnInvertedIndex
{
    List<string>
        Operation(Dictionary<string, List<string>> inverted, List<string> words,List<string> doc);
}
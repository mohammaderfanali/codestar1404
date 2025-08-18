namespace SearchEngine;

public interface IPhraseSearcher
{
    List<string> Search(Dictionary<string, Dictionary<string, List<int>>> invertedIndex
        , List<string> phrase);
}
using faz3.normaler;

namespace faz3;

public class FullstringWordsToIndex:IWorkOnInvertedIndex
{
    public List<string> Operation(Dictionary<string, Dictionary<string, List<int>>> invertedIndex, 
        List<string> fullWords, List<string> doc)
    {
        Tokenizer mytokener;
        mytokener = new (new Normalizer());
        
        foreach (var word in fullWords)
        {
            
        }
        return doc;

    }
}
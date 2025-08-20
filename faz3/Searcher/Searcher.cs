namespace faz3;

public class Searcher : ISearcher
{
    private List<string> _docs;
    private Dictionary<string, Dictionary<string, List<int>>> _invertedIndex;
    private ITokenizer _myTokener;
    public Searcher(Dictionary<string, Dictionary<string, List<int>>> inverted,ITokenizer tokenizer)
    {
        _docs = inverted.Values.SelectMany(docs => docs.Keys).Distinct().OrderBy(c=>c).ToList();
        _invertedIndex = inverted;
        _myTokener = tokenizer;
    }
    
    public List<string> Search(string query)
    {
        List<string> required = new();
        List<string> optional = new();
        List<string> excluded = new();
        foreach (var words in _myTokener.Tokenner(query))
        {
            if (words.StartsWith("+"))
            {
                optional.Add(words.Substring(1));
            }
            else if (words.StartsWith("-"))
            {
                excluded.Add(words.Substring(1));
            }
            else required.Add(words);
        }

        AddRequiredWordsToIndex requeirdClass = new();
        AddOptionalWordsToIndex optionalClass = new();
        RemoveFromIndex removeClass = new();
        
        var _copydocs=requeirdClass.Operation(_invertedIndex, required,_docs);
        _copydocs=removeClass.Operation(_invertedIndex,excluded,_copydocs);
        _copydocs=optionalClass.Operation(_invertedIndex, optional,_copydocs);
        return _copydocs;
    }



}

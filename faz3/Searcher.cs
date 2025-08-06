namespace faz3;

public class Searcher : ISearcher
{
    private List<string> _docs;
    private Dictionary<string, List<string>> _invertedIndex;
    private ITokenizer _myTokener;
    public Searcher(Dictionary<string, List<string>> inverted,ITokenizer tokenizer)
    {
        _docs = new List<string>();
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
        
        _docs=requeirdClass.Operation(_invertedIndex, required,_docs);
        _docs=removeClass.Operation(_invertedIndex,excluded,_docs);
        _docs=optionalClass.Operation(_invertedIndex, optional,_docs);
        return _docs;
    }



}

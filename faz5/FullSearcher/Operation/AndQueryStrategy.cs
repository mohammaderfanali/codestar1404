namespace SearchEngine;

public class AndQueryStrategy:IQueryStrategy
{
    private readonly Queryhandler _queryHandler;
    private readonly PhraseSearcher _fullStringSearcher;
    private readonly Dictionary<string, Dictionary<string, List<int>>> _invertedIndex;
    private readonly List<string> _allDocs;

    public AndQueryStrategy(Queryhandler queryhandler, PhraseSearcher fullStringSearcher,
        Dictionary<string, Dictionary<string, List<int>>> invertedIndex)
    {
        this._queryHandler = queryhandler;
        this._fullStringSearcher = fullStringSearcher;
        this._invertedIndex = invertedIndex;
        this._allDocs = invertedIndex.Values.SelectMany(x => x.Keys).ToList();
    }

    public List<string> Execute(string query)
    {
        var queryClauses = _queryHandler.SplitCommandWithRegex(query);
        List<string> requiredTerms = new List<string>();
        foreach (var (value, prefix) in queryClauses)
        {
            if (prefix != '+' && prefix != '-')
            {
                requiredTerms.Add(value);
            }
        }

        var resultDocs = _allDocs;
        foreach (var term in requiredTerms)
        {
            var listOfDocsWithThisText = _fullStringSearcher.Search(_invertedIndex,
                term.Split(' ',StringSplitOptions.RemoveEmptyEntries).ToList());
            resultDocs=resultDocs.Intersect(listOfDocsWithThisText).ToList();
        }
        
        return resultDocs;
    }
}
    
    
    
    

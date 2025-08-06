namespace SearchEngine;


public class OrQueryStrategy:IQueryStrategy
{
    private readonly Queryhandler _queryHandler;
    private readonly PhraseSearcher _fullStringSearcher;
    private readonly Dictionary<string, Dictionary<string, List<int>>> _invertedIndex;
    private readonly List<string> _allDocs;

    public OrQueryStrategy(Queryhandler queryhandler, PhraseSearcher fullStringSearcher,
        Dictionary<string, Dictionary<string, List<int>>> invertedIndex)
    {
        this._queryHandler = queryhandler;
        this._fullStringSearcher = fullStringSearcher;
        this._invertedIndex = invertedIndex;
        _allDocs = invertedIndex.Values.SelectMany(x => x.Keys).ToList();
    }

    public List<string> Execute(string query)
    {
        var queryClauses = _queryHandler.SplitCommandWithRegex(query);
        List<string> optionalTerms = new List<string>();
        foreach (var (value, prefix) in queryClauses)
        {
            if (prefix == '+' )
            {
                optionalTerms.Add(value);
            }
        }
        var unionDocs = new List<string>();
        foreach (var term in optionalTerms)
        {
            var listOfDocsWithThisText = _fullStringSearcher.Search(_invertedIndex,term.Split(' ').ToList());
            unionDocs = unionDocs.Union(listOfDocsWithThisText).ToList();
        }
        if (optionalTerms.Count == 0) return _allDocs;
        return _allDocs.Intersect(unionDocs).ToList();
    }
}



namespace SearchEngine;

public class NotQueryStrategy : IQueryStrategy
{
    private readonly IQueryHandler _queryHandler;
    private readonly IPhraseSearcher _fullStringSearcher;
    private readonly Dictionary<string, Dictionary<string, List<int>>> _invertedIndex;
    private readonly List<string> _allDocs;

    public NotQueryStrategy(IQueryHandler queryhandler, IPhraseSearcher fullStringSearcher,
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
       
        
        List<string> notTerms = new List<string>();

        foreach (var (value, prefix) in queryClauses)
        {
            if (prefix == '-')
            {
                notTerms.Add(value);
            }
        }

        var unionDocs = new List<string>();
        foreach (var term in notTerms)
        {
            var set = _fullStringSearcher.Search(_invertedIndex,term.Split(' ').ToList());
            unionDocs=unionDocs.Union(set).ToList();
        }
        if (notTerms.Count == 0) return _allDocs;
        
        return _allDocs.Except(unionDocs).ToList();
    }
}
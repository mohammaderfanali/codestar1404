namespace SearchEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Searcher : ISearcher
{
    private readonly List<string> _docs;
    private readonly Dictionary<string, Dictionary<string, List<int>>> _invertedIndex;

    private readonly List<IQueryStrategy> _filters;
    public Searcher(Dictionary<string, Dictionary<string, List<int>>> inverted)
    {
        _docs = inverted.Values.SelectMany(docs => docs.Keys).Distinct().OrderBy(c=>c).ToList();
        _invertedIndex = inverted;
        _filters = new();
    }
    
    public List<string> Search(string query)
    {
        var result = _docs;
        foreach (var filter in _filters)
        {
            result=filter.Execute(query).Intersect(result).ToList();
        }
        return result;

    }
    public void addfilter(IQueryStrategy filter)
    {
        _filters.Add(filter);
    }



}

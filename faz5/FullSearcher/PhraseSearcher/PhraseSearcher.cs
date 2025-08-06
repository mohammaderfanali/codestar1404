namespace SearchEngine;

public class PhraseSearcher
{
   
    public List<string> Search(Dictionary<string, Dictionary<string, List<int>>> invertedIndex
        , List<string> phrase)
    {
        var processedPostings  = new Dictionary<string, List<int>>();
        var nextTermPostings =  new Dictionary<string, List<int>>();
        
        for (int i = 0; i < phrase.Count; i++)
        {
            if (!invertedIndex.ContainsKey(phrase[i]))
            {
                return new List<string>();
            }

            if (i == 0)
            {
                processedPostings = invertedIndex[phrase[0]];
                continue;
            }
            nextTermPostings = invertedIndex[phrase[i]];
            processedPostings = processedPostings.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(num => num + 1).ToList()
            );
            processedPostings = IntersectDictionaries(processedPostings,nextTermPostings);
        }
        
        List<string> result=new List<string>();
        foreach (var key in processedPostings.Keys)
        {
            if (processedPostings[key].Count > 0)
            {
                result.Add(key);
            }
        }
        return result;
    }
    
    private Dictionary<string, List<int>> IntersectDictionaries(
        Dictionary<string, List<int>> dict1,
        Dictionary<string, List<int>> dict2)
    {
        var result = new Dictionary<string, List<int>>(); 
        foreach (var key in dict1.Keys.Intersect(dict2.Keys))
        {
            var intersection = dict1[key].Intersect(dict2[key]).ToList();
            if (intersection.Count > 0)
            {
                result[key] = intersection;
            }
        }
        return result;
    }
}
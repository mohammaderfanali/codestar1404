namespace faz3;

public class AddRequiredWordsToIndex : IWorkOnInvertedIndex
{
    public List<string> Operation(Dictionary<string, Dictionary<string, List<int>>> invertedIndex,
        List<string> required, List<string> doc)
    {
        var matchingLists = required
            .Where(word => invertedIndex.ContainsKey(word))
            .Select(word => invertedIndex[word].Keys.ToList())
            .ToList();

        if (matchingLists.Count != required.Count)
        {
            Console.WriteLine(faz3.Massage.no_word);
            return new List<string>();
        }
        else if (matchingLists.Count == 0)  //no required input
        {
            return doc;
        }
        else
        {
            
            var intersectionResult = matchingLists.Aggregate((a, b) => a.Intersect(b).ToList());
            
            return intersectionResult.Intersect(doc).ToList();
        }
    }
    
    public Dictionary<string, List<int>> IntersectDictionaries(
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
    
    
    public List<string> Operation2(Dictionary<string, Dictionary<string, List<int>>> invertedIndex,
        List<List<string>> requireds, List<string> doc)
    {
        foreach (var required in requireds)
        {
            for (int i = 0; i < required.Count; i++)
            {
                if (!invertedIndex.ContainsKey(required[i]))
                {
                    return new List<string>();
                }

                Dictionary<string, List<int>> beforedic=new();
                Dictionary<string, List<int>> nowdic=new();

                if (i == 0)
                {
                    beforedic = invertedIndex[required[i]];
                    continue;
                }
                
                
                nowdic = invertedIndex[required[i]];
                beforedic = beforedic.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Select(num => num + 1).ToList()
                );
                
                beforedic = IntersectDictionaries(beforedic, nowdic);
                if (beforedic.Count == 0)
                {
                    return new List<string>();
                }

                


                if (matchingLists.Count != required.Count)
                {
                    Console.WriteLine(faz3.Massage.no_word);
                    return new List<string>();
                }
                else if (matchingLists.Count == 0) //no required input
                {
                    return doc;
                }
                else
                {

                    var intersectionResult = matchingLists.Aggregate((a, b) => a.Intersect(b).ToList());

                    return intersectionResult.Intersect(doc).ToList();
                }
            }
        }
    }
}
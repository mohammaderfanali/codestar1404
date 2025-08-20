namespace faz3;

public class FullStringSearcher
{
    private Dictionary<string, List<int>> beforeDic;
    private Dictionary<string, List<int>> nowDic;
    public FullStringSearcher()
    {
         beforeDic=new();
         nowDic=new();
    }
    
    
    public List<string> Search(Dictionary<string, Dictionary<string, List<int>>> invertedIndex
        , List<string> fullWords)
    {
        for (int i = 0; i < fullWords.Count; i++)
        {
            if (!invertedIndex.ContainsKey(fullWords[i]))
            {
                return new List<string>();
            }

            if (i == 0)
            {
                beforeDic = invertedIndex[fullWords[0]];
                continue;
            }
            nowDic = invertedIndex[fullWords[i]];
            beforeDic = beforeDic.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(num => num + 1).ToList()
            );
                
            beforeDic = IntersectDictionaries(beforeDic, nowDic);
        }
        
        List<string> result=new List<string>();
        foreach (var key in beforeDic.Keys)
        {
            if (beforeDic[key].Count > 0)
            {
                result.Add(key);
            }
        }

        return result;
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
}
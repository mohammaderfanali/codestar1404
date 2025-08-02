namespace faz3;

public class AddRequiredWordsToIndex : IWorkOnInvertedIndex
{
    public List<string> Operation(Dictionary<string, List<string>> invertedIndex,
        List<string> required, List<string> doc)
    {
        var matchingLists = required
            .Where(word => invertedIndex.ContainsKey(word))
            .Select(word => invertedIndex[word])
            .ToList();

        if (matchingLists.Count != required.Count || required.Count == 0)
        {
            Console.WriteLine(faz3.Massage.no_word);
            return doc;
        }
        else
        {
            
            var intersectionResult = matchingLists.Aggregate((a, b) => a.Intersect(b).ToList());
            
            return intersectionResult.Union(doc).ToList();
        }
    }
}
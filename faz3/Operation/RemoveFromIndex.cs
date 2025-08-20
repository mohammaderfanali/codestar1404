namespace faz3;

public class RemoveFromIndex : IWorkOnInvertedIndex
{
    public List<string> Operation(Dictionary<string, Dictionary<string, List<int>>> invertedIndex, 
        List<string> excluded, List<string> doc)
    {
        foreach (var word in excluded)
        {
            if (invertedIndex.ContainsKey(word))
                doc = doc.Except(invertedIndex[word].Keys.ToList()).ToList();
        }
        return doc;

    }

}
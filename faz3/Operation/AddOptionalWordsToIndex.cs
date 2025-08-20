namespace faz3;

public class AddOptionalWordsToIndex : IWorkOnInvertedIndex
{

    public List<string> Operation(Dictionary<string, Dictionary<string, List<int>>> invertedIndex,
        List<string> optional, List<string> doc)
    {
        if (optional.Count > 0)
        {
            var optionalDocs = optional
                .Where(word => invertedIndex.ContainsKey(word))
                .SelectMany(word => invertedIndex[word].Keys)
                .Distinct()
                .ToList();

            return doc.Intersect(optionalDocs).ToList();
        }
        else return doc;
    }
}



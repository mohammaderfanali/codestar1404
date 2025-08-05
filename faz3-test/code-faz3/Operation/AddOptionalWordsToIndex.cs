namespace faz3;

public class AddOptionalWordsToIndex : IWorkOnInvertedIndex
{

    public List<string> Operation(Dictionary<string, List<string>> invertedIndex,
        List<string> optional, List<string> doc)
    {
        if (optional.Count > 0)
        {
            var optionalDocs = optional
                .Where(word => invertedIndex.ContainsKey(word))
                .SelectMany(word => invertedIndex[word])
                .Distinct()
                .ToList();

            return doc.Intersect(optionalDocs).ToList();
        }
        else return doc;
    }
}



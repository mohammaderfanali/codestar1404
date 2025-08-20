namespace  SearchEngine;
public class InvertedIndexBuilder : IInvertedIndexBuilder
{
    public Dictionary<string, Dictionary<string, List<int>>>  Build(Dictionary<string, string> files,ITokenizer tokenizer)
    {
        var invertedIndex = new Dictionary<string, Dictionary<string, List<int>>>();
        foreach (var file in files)
        {
            var words = tokenizer.Tokenize(file.Value);
            for (var i = 0; i < words.Count; i++)
            {
                var word = words[i];
                if (!invertedIndex.ContainsKey(word)){
                    invertedIndex[word] = new();
                }
                if (!invertedIndex[word].ContainsKey(file.Key))
                {
                    invertedIndex[word][file.Key] = new List<int>();
                } 
                invertedIndex[word][file.Key].Add(i + 1); // Assuming positions start at 1
            }
        }
        return invertedIndex;
    }
}
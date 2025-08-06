using InvertedIndex;

namespace faz3;

public class AddToInvertedIndex : IAddToInvertedIndex
{
    public  Dictionary<string, List<string>> InvertedIndex { get; }

    public AddToInvertedIndex()
    {
        InvertedIndex = new Dictionary<string, List<string>>();
        
    }
    public void MakeInvertedindex(Dictionary<string, string> files,ITokenizer tokenizer)
    {
        foreach (var file in files)
        {
            var words = tokenizer.Tokenner(file.Value);
            foreach (var word in words)
            {
                if (!InvertedIndex.ContainsKey(word))
                    InvertedIndex[word] = new List<string>();

                if (!InvertedIndex[word].Contains(file.Key))
                    InvertedIndex[word].Add(file.Key);
            }
        }
    }
}
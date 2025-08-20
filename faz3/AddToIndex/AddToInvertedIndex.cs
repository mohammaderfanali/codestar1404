using InvertedIndex;

namespace faz3;

public class AddToInvertedIndex : IAddToInvertedIndex
{
    public  Dictionary<string, Dictionary<string, List<int>>> InvertedIndex { get; }

    public AddToInvertedIndex()
    {
        InvertedIndex = new ();
        
    }
    public void MakeInvertedindex(Dictionary<string, string> files,ITokenizer tokenizer)
    {
        foreach (var file in files)
        {
            var words = tokenizer.Tokenner(file.Value);
            for (var i = 0; i < words.Count; i++)
            {
                var word = words[i];
                if (!InvertedIndex.ContainsKey(word)){
                    InvertedIndex[word] = new();
                }
                
                if (!InvertedIndex[word].ContainsKey(file.Key))
                {
                    InvertedIndex[word][file.Key] = new List<int>();
                } 
                InvertedIndex[word][file.Key].Add(i + 1); // Assuming positions start at 1

                
            }
        }
    }
}
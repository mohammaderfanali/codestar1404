namespace SearchEngine;

public interface IInvertedIndexBuilder
{
     Dictionary<string, Dictionary<string, List<int>>>  Build(Dictionary<string, string> files, ITokenizer tokenizer);
}
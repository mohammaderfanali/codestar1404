using faz3;

namespace InvertedIndex;

public interface IAddToInvertedIndex
{
    void MakeInvertedindex(Dictionary<string, string> files, ITokenizer tokenizer);
}
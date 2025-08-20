using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text.Json;
namespace faz3;
using faz3.normaler;

public class Tokenizer:ITokenizer
{
    private readonly Normalizer normalizer;

    public Tokenizer(Normalizer normalizer)
    {
        this.normalizer = normalizer;
    }
    
    
    public List<string> Tokenner(string content)
    {
        string jsonChars = faz3.Separators.SpecialCharacters;
        var separators = JsonSerializer.Deserialize<List<char>>(jsonChars);
        char[] separatorArray = separators.ToArray();
        var normalized = normalizer.Normalize(content);
        var words = normalized.Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);
            
        return words.ToList();
    }
    }

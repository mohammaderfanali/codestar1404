using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text.Json;
namespace SearchEngine;

public class Tokenizer:ITokenizer
{
    private readonly INormalizer normalizer;
    private readonly char[] separatorArray;

    public Tokenizer(INormalizer normalizer)
    {
        this.normalizer = normalizer;
        string jsonChars = SearchEngine.Separators.SpecialCharacters;
        var separators = JsonSerializer.Deserialize<List<char>>(jsonChars);
        separatorArray = separators.ToArray();
    }
    
    
    public List<string> Tokenize(string content)
    {
        var normalized = normalizer.Normalize(content);
        var words = normalized.Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);
        return words.ToList();
    }
    }

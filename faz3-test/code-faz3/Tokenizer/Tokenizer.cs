using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text.Json;
namespace faz3;


public class Tokenizer:ITokenizer
{
    
    
    public List<string> Tokenner(string content)
    {
        string jsonChars = faz3.Separators.SpecialCharacters;
        var separators = JsonSerializer.Deserialize<List<char>>(jsonChars);
        char[] separatorArray = separators.ToArray();
        var words = content.Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);
            
        return words.ToList();
    }
    }

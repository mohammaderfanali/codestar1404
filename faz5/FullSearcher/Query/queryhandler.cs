using System.Text.RegularExpressions;
namespace SearchEngine;
public class Queryhandler
{
    
    public List<(string value, char prefix)> SplitCommandWithRegex(string input)
    {
        var tokens = new List<(string, char)>();
        // Pattern to match:
        // 1. Optional + or - before quoted strings
        // 2. Regular quoted strings
        // 3. +prefixed words
        // 4. -prefixed words 
        // 5. Normal words
        
        string pattern = @"(?<prefix>[+-])?(?<quote>"".*?"")|(?<plus>\+[^""\s]+)|(?<minus>-[^""\s]+)|(?<word>[^""\s]+)";
        
        foreach (Match match in Regex.Matches(input.ToLower(), pattern))
        {
            char prefix = '\0'; // default no prefix
            
            if (match.Groups["prefix"].Success && match.Groups["quote"].Success)
            {
                // Case: +"..." or -"..."
                prefix = match.Groups["prefix"].Value[0];
                tokens.Add((match.Groups["quote"].Value.Trim('"'), prefix));
            }
            else if (match.Groups["quote"].Success)
            {
                // Case: regular "..."
                tokens.Add((match.Groups["quote"].Value.Trim('"'), prefix));
            }
            else if (match.Groups["plus"].Success)
            {
                // Case: +word
                tokens.Add((match.Groups["plus"].Value[1..], '+'));
            }
            else if (match.Groups["minus"].Success)
            {
                // Case: -word
                tokens.Add((match.Groups["minus"].Value[1..], '-'));
            }
            else if (match.Groups["word"].Success)
            {
                // Case: normal word
                tokens.Add((match.Groups["word"].Value, prefix));
            }
        }

        return tokens;
    }
}
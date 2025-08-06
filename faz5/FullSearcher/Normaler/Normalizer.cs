namespace SearchEngine;
using System.Text.RegularExpressions;
public class Normalizer : INormalizer
{
    public string Normalize(string text)
    {
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return Regex.Replace(text.ToLower(), @"[^\w\s]", "");
        }
    }
}
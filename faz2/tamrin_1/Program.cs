using System;
using System.Collections.Generic;
using System.IO;

class InvertedIndexFromPlainText
{
    static void Main()
    {
        string folderPath = @"C:\Users\soroo\Downloads\the-20-newsgroups\EnglishData"; // Adjust this to your folder path

        var invertedIndex = new Dictionary<string, List<string>>();

        foreach (var filePath in Directory.GetFiles(folderPath))
        {
            string content = File.ReadAllText(filePath).ToLower();
            string fileName = Path.GetFileName(filePath);

            var words = content.Split(new[] {
                ' ', '.', ',', '?', ':', ';', '-', '_', '\n', '\r',
                '(', ')', '[', ']', '{', '}'
            }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                if (!invertedIndex.ContainsKey(word))
                    invertedIndex[word] = new List<string>();

                if (!invertedIndex[word].Contains(fileName))
                    invertedIndex[word].Add(fileName);
            }
        }

        // Search for a word
        Console.Write("Enter a word to search: ");
        string query = Console.ReadLine()?.ToLower() ?? "";

        if (invertedIndex.ContainsKey(query))
        {
            Console.WriteLine($"✅ Word '{query}' appears in the following files:");
            foreach (var doc in invertedIndex[query])
                Console.WriteLine($"- {doc}");
        }
        else
        {
            Console.WriteLine($"❌ Word '{query}' was not found in any files.");
        }
    }
}

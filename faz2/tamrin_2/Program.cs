using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

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
        List<string> mos = new List<string>();
        List<string> man = new List<string>();
        List<string> nor = new List<string>();

        foreach (var i in query.Split(' '))
        {
            if (i[0] == '+') mos.Add(i.Substring(1));
            else if (i[0] == '-') man.Add(i.Substring(1));
            else nor.Add(i);
            Console.WriteLine($"i is {i}");
        }
        foreach (var item in mos)
            Console.WriteLine(item);

        foreach (var item in man)
            Console.WriteLine(item);

        foreach (var item in nor)
            Console.WriteLine(item);



        var intersectedChars = nor
        .Select(word => invertedIndex[word].ToList())
        .Aggregate((prev, next) => prev.Intersect(next).ToList());

        Console.WriteLine(string.Join(", ", intersectedChars));

        foreach (var s in man)
        {
            intersectedChars = intersectedChars.Except(invertedIndex[s]).ToList();
        }
        Console.WriteLine("after intersection-----------------");
        Console.WriteLine(string.Join(", ", intersectedChars));
        Console.WriteLine("after except-----------------");


        List<string> union = new List<string>();

        foreach (var s in mos)
        {
            union = union.Union(invertedIndex[s]).ToList();

        }

        intersectedChars = intersectedChars.Intersect(union).ToList();

        Console.WriteLine(string.Join(", ", intersectedChars));
        Console.WriteLine("final-----------------");


    }
}

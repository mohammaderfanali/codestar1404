namespace faz3;
using file;
using System.Text.Json;
using System.IO;

#pragma warning disable CS0219

public class Application: IApplication
{
    public void Run()
    {
        DocsReader myFile = new ();
        Dictionary<string, string> allFile = myFile.ReadFiles(faz3.paths.docs);
        Tokenizer myTokener = new Tokenizer();
        AddToInvertedIndex myInvertedIndex = new ();
        myInvertedIndex.MakeInvertedindex(allFile,myTokener);

        Console.Write("input:");
        string? query = Console.ReadLine();

        Searcher mySearch = new (myInvertedIndex.InvertedIndex,myTokener);
        Console.WriteLine(string.Join(", ", mySearch.Search(query ?? "")));

    }
}
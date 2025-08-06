namespace  SearchEngine;
using SearchEngine;
using System.Text.Json;
using System.IO;

#pragma warning disable CS0219
public class Application: IApplication
{
    public void Run()
    {
        FileReader myFile = new ();
        var allFile = myFile.ReadFiles(SearchEngine.paths.docs);
        var myTokener = new Tokenizer(new Normalizer());
        var myInvertedIndex = new InvertedIndexBuilder();
        var invertedIndex = myInvertedIndex.Build(allFile,myTokener);

        Console.Write("input:");
        string? query = Console.ReadLine();


        var queryHandler =new QueryHandler();
        var fullStringSearcher = new PhraseSearcher();
        var mySearch = new Searcher(invertedIndex);
        var optionals = new OrQueryStrategy(queryHandler,fullStringSearcher,invertedIndex);
        var requierd = new AndQueryStrategy(queryHandler,fullStringSearcher,invertedIndex);
        var excluded = new NotQueryStrategy(queryHandler,fullStringSearcher,invertedIndex);
        mySearch.addfilter(optionals);
        mySearch.addfilter(requierd);
        mySearch.addfilter(excluded);

        
        Console.WriteLine(string.Join(", ", mySearch.Search(query ?? "")));

    }
}
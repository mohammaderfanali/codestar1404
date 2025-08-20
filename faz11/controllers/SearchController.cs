using Microsoft.AspNetCore.Mvc;
namespace SearchEngine;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly Searcher _searcher;

    public SearchController()
    {
        FileReader myFile = new();
        var allFile = myFile.ReadFiles(faz11.paths.docs);
        var myTokener = new Tokenizer(new Normalizer());
        var myInvertedIndex = new InvertedIndexBuilder();
        var invertedIndex = myInvertedIndex.Build(allFile, myTokener);

        var queryHandler = new QueryHandler();
        var fullStringSearcher = new PhraseSearcher();

        var optionals = new OrQueryStrategy(queryHandler, fullStringSearcher, invertedIndex);
        var requierd = new AndQueryStrategy(queryHandler, fullStringSearcher, invertedIndex);
        var excluded = new NotQueryStrategy(queryHandler, fullStringSearcher, invertedIndex);

        _searcher = new Searcher(invertedIndex);
        _searcher.addfilter(optionals);
        _searcher.addfilter(requierd);
        _searcher.addfilter(excluded);
    }

    [HttpGet]
    public IActionResult Search([FromQuery] string query)
    {
        Console.WriteLine(query);
        var result = _searcher.Search(query ?? "");
        return Ok(result);
    }
}
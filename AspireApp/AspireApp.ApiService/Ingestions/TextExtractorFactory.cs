namespace AspireApp.ApiService.Ingestion;

// Picks the right ITextExtractor for a given file at runtime.
public class TextExtractorFactory(IEnumerable<ITextExtractor> extractors)
{
    public ITextExtractor GetExtractor(string fileName)
    {
        var extractor = extractors.FirstOrDefault(e => e.CanHandle(fileName));

        return extractor
            ?? throw new NotSupportedException($"No text extractor registered for file: {fileName}");
    }
}
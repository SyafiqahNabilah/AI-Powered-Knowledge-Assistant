using UglyToad.PdfPig; // yes, that's really PdfPig's root namespace

namespace AspireApp.ApiService.Ingestion;

public class PdfTextExtractor : ITextExtractor
{
    public bool CanHandle(string fileName) =>
        fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);

    public Task<string> ExtractTextAsync(byte[] fileBytes, CancellationToken ct = default)
    {
        // PdfPig's parsing is CPU-bound, not I/O-bound, so there's nothing to
        // genuinely await here. We wrap the result in Task.FromResult so this
        // method still satisfies the shared async ITextExtractor contract —
        // callers don't need to know or care that this particular implementation
        // happens to run synchronously under the hood.
        using var document = PdfDocument.Open(fileBytes);

        var pageTexts = document.GetPages().Select(page => page.Text);
        return Task.FromResult(string.Join("\n\n", pageTexts));
    }
}
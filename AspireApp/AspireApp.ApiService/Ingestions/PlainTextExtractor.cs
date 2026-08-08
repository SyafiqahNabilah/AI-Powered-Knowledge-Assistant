using System.Text;

namespace AspireApp.ApiService.Ingestion;

// Handles .md and .txt — no real "extraction" needed, just UTF-8 decoding.
// We deliberately don't strip Markdown syntax; the LLM handles Markdown fine,
// and the structure (headings, lists) can actually help it understand the content.
public class PlainTextExtractor : ITextExtractor
{
    public bool CanHandle(string fileName) =>
        fileName.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
        fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);

    public Task<string> ExtractTextAsync(byte[] fileBytes, CancellationToken ct = default) =>
        Task.FromResult(Encoding.UTF8.GetString(fileBytes));
}
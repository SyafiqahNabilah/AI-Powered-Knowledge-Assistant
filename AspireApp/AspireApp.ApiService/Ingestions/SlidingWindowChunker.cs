namespace AspireApp.ApiService.Ingestion;

// Sliding-window chunker: splits text into overlapping word-count windows.
//
// DESIGN DECISION — worth restating in your project's design-decisions doc:
// We chunk by *word count*, not token count. A real tokenizer (tiktoken-style)
// sizes chunks more precisely relative to the embedding model's actual token
// limit, but it's an extra dependency and complexity that isn't worth it for
// a portfolio project. Word count is a reasonable approximation (roughly
// 0.75 words per token for English text), and the overlap protects against
// losing context right at a chunk boundary.
public class SlidingWindowChunker(int chunkSizeInWords = 300, int overlapInWords = 50) : ITextChunker
{
    public IReadOnlyList<TextChunk> Chunk(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        // Passing `null` as the separator tells Split to treat any whitespace
        // run as a delimiter. TrimEntries + RemoveEmptyEntries cleans up the
        // stray empty strings that double spaces/newlines would otherwise leave.
        var words = text.Split(
            (char[]?)null,
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var step = chunkSizeInWords - overlapInWords;
        if (step <= 0)
            throw new ArgumentException("Overlap must be smaller than chunk size.");

        var chunks = new List<TextChunk>();
        var index = 0;

        for (var start = 0; start < words.Length; start += step)
        {
            var windowWords = words.Skip(start).Take(chunkSizeInWords).ToArray();
            if (windowWords.Length == 0)
                break;

            chunks.Add(new TextChunk(string.Join(' ', windowWords), index++));

            // If this window already reached the end of the text, stop — otherwise
            // the overlap math produces a near-duplicate trailing chunk.
            if (start + chunkSizeInWords >= words.Length)
                break;
        }

        return chunks;
    }
}
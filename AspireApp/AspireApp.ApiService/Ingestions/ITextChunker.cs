namespace AspireApp.ApiService.Ingestion;

public interface ITextChunker
{
    IReadOnlyList<TextChunk> Chunk(string text);
}
namespace Cortex.Api.Ingestion;

public interface ITextChunker
{
    IReadOnlyList<TextChunk> Chunk(string text);
}
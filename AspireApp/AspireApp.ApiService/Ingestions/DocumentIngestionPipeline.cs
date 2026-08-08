using AspireApp.ApiService.Data;
using AspireApp.ApiService.Documents;
using Microsoft.Extensions.AI;
using Pgvector;

namespace AspireApp.ApiService.Ingestion;

public class DocumentIngestionPipeline(
    TextExtractorFactory extractorFactory,
    ITextChunker chunker,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    ApplicationDbContext db,
    ILogger<DocumentIngestionPipeline> logger)
{
    public async Task IngestAsync(Document document, byte[] fileBytes, CancellationToken ct = default)
    {
        var extractor = extractorFactory.GetExtractor(document.FileName);
        var text = await extractor.ExtractTextAsync(fileBytes, ct);

        var textChunks = chunker.Chunk(text);
        logger.LogInformation(
            "Document {DocumentId} ({FileName}) split into {ChunkCount} chunks",
            document.Id, document.FileName, textChunks.Count);

        foreach (var textChunk in textChunks)
        {
            // GenerateAsync's native shape is batch-first: IEnumerable<string> in,
            // a matching batch of embeddings out. We're passing a batch of one here
            // for clarity — sending ALL of a document's chunks in a single call is a
            // real optimization worth doing once this works end-to-end (far fewer
            // network round trips to Ollama). Some versions of Microsoft.Extensions.AI
            // also expose a singular convenience method (e.g. GenerateEmbeddingVectorAsync)
            // — functionally equivalent, use whichever your IDE shows.
            var embeddings = await embeddingGenerator.GenerateAsync([textChunk.Content], cancellationToken: ct);
            var embeddingVector = embeddings.First().Vector.ToArray();

            var chunk = new DocumentChunck
            {
                DocumentId = document.Id,
                Content = textChunk.Content,
                ChunkIndex = textChunk.Index,
                // Pgvector.EntityFrameworkCore's Vector type wraps a float[] so EF Core
                // knows how to serialize it to the `vector` Postgres column.
                // Note: there's also a System.Numerics.Vector — if you get a namespace
                // ambiguity error, fully qualify this as Pgvector.Vector.
                Embedding = new Vector(embeddingVector),
            };

            db.Chunks.Add(chunk);
        }

        await db.SaveChangesAsync(ct);
    }
}
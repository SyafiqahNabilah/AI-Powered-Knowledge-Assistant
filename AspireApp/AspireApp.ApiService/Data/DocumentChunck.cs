using Pgvector;

namespace AspireApp.ApiService.Data;

public class DocumentChunck
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid DocumentId { get; set; }
    public Document? Document { get; set; }

    public required string Content { get; set; }
    public required int ChunkIndex { get; set; }   // preserves original order within the source document

    public required Vector Embedding { get; set; }   // from Pgvector.EntityFrameworkCore
}
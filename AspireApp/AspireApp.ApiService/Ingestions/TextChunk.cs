namespace AspireApp.ApiService.Ingestion;

// `record` gives value-based equality and a free ToString() for a small immutable
// DTO like this — the C# equivalent of a Java 16+ record or a Lombok @Value class.
public record TextChunk(string Content, int Index);
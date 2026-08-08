 namespace Cortex.Api.Ingestion;

 // Small abstraction so callers don't need to know *how* text was extracted —
 // same reasoning as coding against an interface rather than a concrete class in Java.
 public interface ITextExtractor
 {
     // Returns true if this extractor knows how to handle the given file name.
     bool CanHandle(string fileName);

     Task<string> ExtractTextAsync(byte[] fileBytes, CancellationToken ct = default);
 }
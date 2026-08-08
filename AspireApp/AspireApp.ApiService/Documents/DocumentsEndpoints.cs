using System.Security.Cryptography;
using AspireApp.ApiService.Data;
using AspireApp.ApiService.Ingestion;
using Microsoft.EntityFrameworkCore;

namespace AspireApp.ApiService.Documents;

// Extension-method pattern: groups all "/documents" routes in one place without
// needing a controller class. Program.cs just calls app.MapDocumentsEndpoints().
public static class DocumentsEndpoints
{
    public static void MapDocumentsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/documents").WithTags("Documents");
        group.MapPost("/", UploadDocument).DisableAntiforgery();
    }

    // IFormFile = ASP.NET Core's MultipartFile equivalent. The runtime handles the
    // multipart/form-data parsing for us; we just declare the parameter type.
   private static async Task<IResult> UploadDocument(
       IFormFile file,
       ApplicationDbContext db,
       DocumentIngestionPipeline pipeline,   // newly injected
       CancellationToken ct)
   {
       if (file.Length == 0)
           return Results.BadRequest("File is empty.");

       using var memoryStream = new MemoryStream();
       await file.CopyToAsync(memoryStream, ct);
       var fileBytes = memoryStream.ToArray();

       var contentHash = Convert.ToHexString(SHA256.HashData(fileBytes));

       var existing = await db.Documents.FirstOrDefaultAsync(d => d.ContentHash == contentHash, ct);
       if (existing is not null)
           return Results.Ok(new { existing.Id, Message = "Document already ingested." });

       var document = new Document
       {
           FileName = file.FileName,
           ContentHash = contentHash,
       };

       db.Documents.Add(document);
       await db.SaveChangesAsync(ct);

       try
       {
           // Synchronous within the request for now — the caller waits until every
           // chunk is embedded before getting a response. Fine at portfolio scale;
           // a production system would return immediately and process in a background
           // job (IHostedService or a message queue), then let the client poll or get
           // notified when ingestion finishes.
           await pipeline.IngestAsync(document, fileBytes, ct);
       }
       catch (NotSupportedException ex)
       {
           // We already saved the Document row before extraction could fail. Roll it
           // back so a retry with a supported file type doesn't collide with the
           // ContentHash uniqueness check above.
           db.Documents.Remove(document);
           await db.SaveChangesAsync(ct);
           return Results.BadRequest(ex.Message);
       }

       return Results.Created($"/documents/{document.Id}", document);
   }
}
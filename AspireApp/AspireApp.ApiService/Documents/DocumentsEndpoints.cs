using System.Security.Cryptography;
using AspireApp.ApiService.Data;
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
        CancellationToken ct)
    {
        if (file.Length == 0)
            return Results.BadRequest("File is empty.");

        // Read the whole file into memory. Fine at portfolio scale (PDFs/notes);
        // a production system handling large files would stream instead of
        // buffering the whole thing.
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, ct);
        var fileBytes = memoryStream.ToArray();

        // Content hashing gives us idempotent ingestion for free: re-uploading the
        // exact same bytes returns the existing Document instead of reprocessing
        // and creating duplicate chunks. This is the "idempotent ingestion" senior
        // note from the roadmap, implemented here rather than bolted on later.
        var contentHash = Convert.ToHexString(SHA256.HashData(fileBytes));

        var existing = await db.Documents
            .FirstOrDefaultAsync(d => d.ContentHash == contentHash, ct);

        if (existing is not null)
            return Results.Ok(new { existing.Id, Message = "Document already ingested." });

        var document = new Document
        {
            FileName = file.FileName,
            ContentHash = contentHash,
        };

        db.Documents.Add(document);
        await db.SaveChangesAsync(ct); // save now so document.Id is valid before we touch chunks

        // Extraction/chunking/embedding gets wired in here in step 3.5, once those
        // pieces exist. For now this just proves upload -> DB round trip works.
        return Results.Created($"/documents/{document.Id}", document);
    }
}
using System;
using System.Collections.Generic;

using AspireApp.ApiService.Data;

namespace AspireApp.ApiService.Data;

public class Document
{
    public Guid Id { get; set; }
    public required string FileName { get; set; }
    public required string ContentHash { get; set; } // SHA-256 of file bytes — enables idempotent re-upload checks in Phase 3
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<DocumentChunck> Chunks { get; set; } = [];
}
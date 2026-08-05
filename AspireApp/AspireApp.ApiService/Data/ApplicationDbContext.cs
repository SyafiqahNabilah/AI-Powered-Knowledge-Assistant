using Microsoft.EntityFrameworkCore;


namespace AspireApp.ApiService.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : DbContext(options)
{
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentChunck> Chunks => Set<DocumentChunck>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector"); // generates `CREATE EXTENSION IF NOT EXISTS vector` in the migration

        modelBuilder.Entity<DocumentChunck>()
            .Property(c => c.Embedding)
            .HasColumnType("vector(768)"); // matches nomic-embed-text's output dimension — change if you picked a different model

        modelBuilder.Entity<DocumentChunck>()
            .HasOne(c => c.Document)
            .WithMany(d => d.Chunks)
            .HasForeignKey(c => c.DocumentId)
            .OnDelete(DeleteBehavior.Cascade); // deleting a Document deletes its Chunks — no orphaned rows

        base.OnModelCreating(modelBuilder);
    }
}
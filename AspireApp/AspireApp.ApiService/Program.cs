using AspireApp.ApiService.Data;
using Microsoft.EntityFrameworkCore;
using AspireApp.ApiService.Documents;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Add database context for PostgreSQL
builder.AddNpgsqlDbContext<ApplicationDbContext>("cortexdb", configureDbContextOptions: options =>
{
    options.UseNpgsql(o => o.UseVector()); // teaches Npgsql about pgvector's wire format
});

// Both extractors register against the same interface. Injecting IEnumerable<ITextExtractor>
// (which TextExtractorFactory does via its primary constructor) resolves *every* registered
// implementation — the direct equivalent of Spring auto-wiring List<TextExtractor> against
// every bean implementing that interface.
builder.Services.AddSingleton<ITextExtractor, PdfTextExtractor>();
builder.Services.AddSingleton<ITextExtractor, PlainTextExtractor>();
builder.Services.AddSingleton<TextExtractorFactory>();

var app = builder.Build();
//migration execution
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "API service is running. Navigate to /documents to see sample data.");
app.MapDocumentsEndpoints(); // wire up the /documents endpoints defined in DocumentsEndpoints.cs
app.MapDefaultEndpoints();

app.Run();

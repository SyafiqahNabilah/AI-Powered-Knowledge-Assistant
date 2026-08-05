# AI-Powered-Knowledge-Assistant

A internal-docs / notes assistant where users upload documents and chat with an AI that answers using retrieval-augmented generation (RAG) over their own content.

# Project 1: AI-Powered Knowledge Assistant — Build Roadmap

Written for a Java developer with zero .NET experience. Keep this file in your repo and check off tasks (`- [ ]` → `- [x]`) as you go — it doubles as a progress log for anyone reviewing your commit history.

---

## Before You Start: Java → .NET Cheat Sheet

## Phase 0 — Environment Setup

_(no commits yet — this is local machine setup)_

- [x] Install the **.NET 10 SDK**, verify with `dotnet --version`
- [x] Install **Docker Desktop** (Aspire uses it to spin up Postgres)
- [x] Install an IDE: **VS Code + "C# Dev Kit" extension** (cross-platform, closest to a lightweight IntelliJ setup), or **Rider** if you have a JetBrains license (very natural coming from IntelliJ), or **Visual Studio** on Windows
- [x] Create the GitHub repo, clone it locally, add a `.gitignore` (use the "VisualStudio" GitHub template) and a license

**Commit:** `chore: initialize repository with .gitignore and license`

---

## Phase 1 — Solution Scaffolding

- [x] Scaffold the **.NET Aspire** solution: an **AppHost** project (the orchestrator — think a docker-compose file with a supervisor attached), a **ServiceDefaults** project (shared cross-cutting config: telemetry, health checks, resilience — like a shared internal Spring Boot starter), and an empty **API** project.

  > ⚠️ Aspire's exact `dotnet new` template names shift between versions — check the [current install steps](https://learn.microsoft.com/dotnet/aspire/get-started/aspire-overview) before running this rather than trusting a remembered command.

  **Commit:** `feat: scaffold .NET Aspire solution with AppHost and API project`

- [x] Add the **Blazor Web App** project, reference it from AppHost.

  **Commit:** `feat: add Blazor frontend project`

- [x] Run it (`dotnet run` from AppHost) and confirm the **Aspire dashboard** loads — this is your local control tower: traces, logs, and env vars for every service in one place. Nothing to commit here, just a checkpoint.

- [x] Write a first-pass project README describing the goal and linking back to your portfolio README.

  **Commit:** `docs: add initial project README`

---

## Phase 2 — Data Layer

- [x] Add NuGet packages: `Npgsql.EntityFrameworkCore.PostgreSQL`, `Pgvector.EntityFrameworkCore`

  **Commit:** `chore: add EF Core and pgvector NuGet packages`

- [x] In AppHost, add a Postgres container resource (pgvector-enabled image) and wire it to the API project — Aspire injects the connection string automatically, no manual "wait for DB" scripts needed.

  **Commit:** `feat: add Postgres with pgvector via Aspire hosting`

- [ ] Define entity models: `Document`, `DocumentChunk` (with a vector column for the embedding) — your `@Entity` equivalent, just plain C# classes.

  **Commit:** `feat: define Document and DocumentChunk entity models`

- [ ] Create the `DbContext` (your `EntityManager`/DAO equivalent) and register it in DI.

  **Commit:** `feat: add ApplicationDbContext`

- [ ] Create and apply the first migration (`dotnet ef migrations add InitialCreate`, `dotnet ef database update`).

  **Commit:** `feat: add initial database migration`

---

## Phase 3 — Document Ingestion Pipeline

- [ ] Add a Minimal API endpoint for upload (`POST /documents`).

  **Commit:** `feat: add document upload endpoint`

- [ ] Extract raw text from uploaded PDF/Markdown files.

  **Commit:** `feat: extract text from uploaded documents`

- [ ] Implement chunking (e.g. ~500 tokens per chunk, ~50-token overlap) as its own class so it's independently testable.

  **Commit:** `feat: implement text chunking strategy`

- [ ] Add `Microsoft.Extensions.AI` and configure an embedding client (Ollama = free/offline for dev; OpenAI or Azure OpenAI for closer-to-production quality).

  **Commit:** `feat: wire up embedding generation via Microsoft.Extensions.AI`

- [ ] Generate and persist embeddings for each chunk on upload.

  **Commit:** `feat: persist chunk embeddings to Postgres`

- [ ] Unit test the chunking logic — one of the few genuinely pure, I/O-free pieces in this project.

  **Commit:** `test: add unit tests for chunking logic`

---

## Phase 4 — Retrieval + Chat Backend

- [ ] Implement vector similarity search (cosine distance via pgvector) returning top-k nearest chunks.

  **Commit:** `feat: add vector similarity search`

- [ ] Configure an `IChatClient` for the actual chat model.

  **Commit:** `feat: configure chat client`

- [ ] Build the RAG orchestration: embed the question → retrieve top-k chunks → construct grounded prompt → call chat client.

  **Commit:** `feat: implement RAG orchestration pipeline`

- [ ] Track and return source citations with each answer — this detail alone signals "production-minded" rather than "toy demo."

  **Commit:** `feat: add citation tracking to chat responses`

- [ ] Expose a streaming chat endpoint (Server-Sent Events, or SignalR for bidirectional).

  **Commit:** `feat: add streaming chat endpoint`

- [ ] Test it end-to-end with a `.http` file or Postman before touching the frontend.

---

## Phase 5 — Blazor Frontend

- [ ] Build the document upload UI.

  **Commit:** `feat: add document upload UI`

- [ ] Build the chat UI with streaming response rendering.

  **Commit:** `feat: add chat UI with streaming responses`

- [ ] Display citations under each answer.

  **Commit:** `feat: display source citations in chat UI`

- [ ] Connect frontend to backend via Aspire service discovery (a typed `HttpClient` that resolves the API automatically — no hardcoded localhost ports).

  **Commit:** `feat: connect frontend to backend via service discovery`

- [ ] Basic styling pass — clean and legible over beautiful.

  **Commit:** `style: polish chat UI layout and styling`

---

## Phase 6 — Auth

- [ ] Add authentication (ASP.NET Core Identity, or a lighter API-key scheme to keep scope small).

  **Commit:** `feat: add authentication`

- [ ] Scope documents and conversations per user.

  **Commit:** `feat: scope documents and chats per user`

---

## Phase 7 — Observability & Resilience

- [ ] Confirm OpenTelemetry tracing flows end-to-end in the Aspire dashboard; add custom spans around the RAG pipeline specifically.

  **Commit:** `feat: add custom telemetry spans to RAG pipeline`

- [ ] Add health check endpoints for the API and DB.

  **Commit:** `feat: add health checks`

- [ ] Add retry/timeout resilience around AI provider calls (`Microsoft.Extensions.Resilience` / Polly v8) — the AI API is the flakiest external dependency you have.

  **Commit:** `feat: add resilience policies around AI client calls`

---

## Phase 8 — Testing

- [ ] Unit test the RAG orchestration logic (mock the chat client and vector store).

  **Commit:** `test: add unit tests for RAG orchestration`

- [ ] Integration test the full upload → chat flow using `Aspire.Hosting.Testing` against real Postgres.

  **Commit:** `test: add integration test for upload-to-chat flow`

---

## Phase 9 — CI/CD & Deployment

- [ ] Add a GitHub Actions workflow: build + test on every push.

  **Commit:** `ci: add build and test workflow`

- [ ] Add a deployment manifest (Azure Container Apps via `azd`, or plain Docker Compose if you'd rather skip Azure).

  **Commit:** `feat: add deployment manifest for Azure Container Apps`

- [ ] Deploy and smoke-test the live version.

---

## Phase 10 — Documentation & Launch

- [ ] Add an architecture diagram (Excalidraw, draw.io, or Mermaid inline in the README) showing ingestion and chat flows.

  **Commit:** `docs: add architecture diagram`

- [ ] Write a **Design Decisions** section — why Blazor over React, why Postgres+pgvector over a dedicated vector DB, why this chunking strategy. Reviewers read this more than your code.

  **Commit:** `docs: add design decisions section`

- [ ] Record a 2–3 minute demo video/GIF.

  **Commit:** `docs: add demo video link`

- [ ] Final README polish, then tag the release.

  **Commit:** `docs: finalize README`
  **Then:** `git tag v1.0.0`

---

## Senior-Level Things Worth Knowing

These aren't tasks — they're judgment calls that separate "it works" from "I'd trust this in production." Good to actually understand, and good talking points in an interview.

1. **Secrets management** — never put API keys in `appsettings.json` or commit them. Use `dotnet user-secrets` locally; environment variables or Azure Key Vault in production.
2. **DI lifetimes matter** — Scoped vs. Transient vs. Singleton is a classic .NET gotcha. EF Core's `DbContext` defaults to Scoped (per-request). Get this wrong and you'll see confusing bugs from a `DbContext` shared across requests it shouldn't be.
3. **Async all the way** — use `async`/`await` and `Task<T>` consistently. Never call `.Result` or `.Wait()` on a task inside ASP.NET Core — unlike blocking calls on a typical Java thread-per-request server, this can deadlock.
4. **Nullable reference types** — turn on `<Nullable>enable</Nullable>` in your `.csproj`. It's C#'s compile-time answer to `NullPointerException`, closer to Kotlin's null-safety than Java's `Optional`.
5. **Review generated migrations** — check the SQL EF Core generates before applying it, especially on tables with real data. Don't hand-edit migration files after the fact.
6. **Structured logging** — `_logger.LogInformation("Uploaded {DocumentId}", id)`, not string concatenation. Same spirit as SLF4J/Logback conventions.
7. **Prompt injection awareness** — a RAG app retrieves untrusted text and feeds it to a model. A malicious document could contain text trying to hijack instructions. Even a basic mitigation (clear separation of system instructions from retrieved content) is worth calling out in your design doc — interviewers ask about this.
8. **Chunking tradeoffs** — be ready to explain your chunk size/overlap choice. One of the most common RAG interview questions.
9. **Token/cost awareness** — log token usage per request. Be ready to discuss how you'd control cost at scale (caching, smaller models for reranking, etc.).
10. **Idempotent ingestion** — re-uploading the same file shouldn't duplicate chunks. Hash the file and check before processing.
11. **Small, focused commits/PRs** — even solo, use feature branches and PR into `main`. It's what a clean, reviewable history looks like on a team.
12. **Graceful health checks** — matters more in containerized/cloud-native environments than typical monolith deployments; don't skip it just because it feels like busywork.

---

## Suggested Git Workflow

- One branch per phase or feature: `feature/ingestion-pipeline`, `feature/rag-orchestration`, etc.
- Open a PR into `main` even solo — self-review it, then merge. Gives you a clean, explainable history.
- Squash-merge so `main`'s history reads as one commit per feature.
- Tag `v1.0.0` once Phase 10 is done. Tag further versions as you extend the project post-launch.

======================
Good — let's build the data layer step by step. I'll assume you've got the Aspire solution scaffolded from Phase 1 (AppHost, ServiceDefaults, an API project, and a Blazor project). I'll use **Cortex** as the example project name throughout — swap in whatever you actually named it.

One honest flag before we start: Aspire's exact API surface moves fast between versions, and the skill list you saw referenced Aspire 13.4, which is newer than what I can verify in detail. The _concepts_ below are stable; if a method name doesn't match what your IDE's autocomplete shows, trust your IDE over my memory and we'll adjust together.

---

## 2.1 — Add the NuGet Packages

**Concept first.** In Java, you'd add a dependency to `pom.xml` and Maven resolves it. In .NET, `dotnet add package` does the same thing to your `.csproj` — it writes a `<PackageReference>` element and NuGet resolves/downloads it into a local cache (`~/.nuget/packages`, analogous to `~/.m2/repository`).

You need two different _kinds_ of packages here, and the distinction matters:

| Package                                        | What it is                                                                                                                                   |
| ---------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| `Npgsql.EntityFrameworkCore.PostgreSQL`        | The actual EF Core database provider for Postgres — talks SQL over the wire                                                                  |
| `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL` | Aspire's _client integration_ wrapper — auto-wires the connection string from service discovery, plus health checks/telemetry/retry for free |
| `Pgvector.EntityFrameworkCore`                 | Community package (pgvector-dotnet) that teaches EF Core about the `vector` column type                                                      |

In a non-Aspire tutorial you'd only ever see the first row. In an Aspire project, you mostly interact with the second — it depends on the first internally.

**Run this** from your API project's folder (or pass `--project`):

```bash
cd src/Cortex.Api
dotnet add package Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Pgvector.EntityFrameworkCore
```

**Checkpoint:** open `Cortex.Api.csproj` and confirm you see new `<PackageReference Include="..." Version="..." />` lines. That's the whole effect — no magic, just a manifest entry, same mental model as `pom.xml`.

---

## 2.2 — Add Postgres (with pgvector) via the AppHost

**Concept first.** The AppHost project is Aspire's orchestrator — it's C# code that _describes_ your distributed app's topology (what runs, what depends on what) instead of a YAML file like `docker-compose.yml`. When you `dotnet run` the AppHost, it stands up every resource you declared and wires connection strings between them automatically. Nothing is hardcoded — no "the DB is on port 5433," no `.env` file with a copy-pasted connection string.

Add the Postgres hosting package to the **AppHost** project specifically (not the API):

```bash
cd src/Cortex.AppHost
dotnet add package Aspire.Hosting.PostgreSQL
```

Now edit `AppHost/Program.cs`. Plain Postgres doesn't ship with the `vector` extension compiled in, so we use the community `pgvector/pgvector` image, which is Postgres + the extension pre-installed:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("pgvector/pgvector", "pg16")   // postgres 16 + pgvector extension baked in
    .WithDataVolume()                          // persist data across `dotnet run` restarts
    .WithHostPort(5432)                        // pin the port so you can psql/pgAdmin in manually
    .WithPgAdmin();                             // optional: web UI to poke at the DB

var cortexDb = postgres.AddDatabase("cortexdb");

var api = builder.AddProject<Projects.Cortex_Api>("api")
    .WithReference(cortexDb)
    .WaitFor(cortexDb);   // don't start the API until Postgres is actually ready

builder.AddProject<Projects.Cortex_Web>("web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
```

**Java parallel:** think of this file as a hybrid of a `docker-compose.yml` and a Spring `@Configuration` class — it's declaring infrastructure _and_ wiring dependency graph in one place.

`WithDataVolume()` is worth pausing on: without it, your Postgres container is stateless — every `dotnet run` gives you a fresh empty database, which is annoying once you have real test data. With it, data survives restarts (backed by a Docker volume).

Now wire the API project to actually _use_ the connection. In `Cortex.Api/Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(); // from Aspire's ServiceDefaults project: telemetry, health checks, resilience

builder.AddNpgsqlDbContext<ApplicationDbContext>("cortexdb", configureDbContextOptions: options =>
{
    options.UseNpgsql(o => o.UseVector()); // teaches Npgsql about pgvector's wire format
});
```

⚠️ That `configureDbContextOptions` overload combining Aspire + pgvector-dotnet is the trickiest integration point in this whole phase — I'm reasonably but not fully confident in that exact signature for your Aspire version. If it doesn't compile, fall back to this (functionally equivalent, more verbose) pattern instead:

```csharp
builder.Services.AddNpgsqlDataSource(
    builder.Configuration.GetConnectionString("cortexdb")!,
    dataSourceBuilder => dataSourceBuilder.UseVector());

builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
    options.UseNpgsql(sp.GetRequiredService<NpgsqlDataSource>()));
```

Note what you notably _don't_ have anywhere: a connection string in `appsettings.json`. The string `"cortexdb"` above is just a _name_ — Aspire resolves the real host/port/credentials at runtime and injects them as an environment variable into the API process. This is the single biggest mental shift from a typical Spring Boot `application.yml` workflow.

**Checkpoint — run it:**

```bash
cd src/Cortex.AppHost
dotnet run
```

Open the Aspire dashboard link printed in the console. You should see three resources: `postgres`, `api`, `web`. Click on `postgres` — its state should go from "Starting" to a green "Running." Click on `api` and check its environment variables tab; you should see a `ConnectionStrings__cortexdb` entry Aspire injected for you. If `postgres` sits red/unhealthy, check Docker Desktop is actually running — that's the #1 cause.

**Commit:** `feat: add Postgres with pgvector via Aspire hosting`

---

## 2.3 — Define the Entity Models

**Concept first.** In JPA/Hibernate you'd annotate a class with `@Entity`, `@Id`, `@Column`. EF Core defaults to _convention over configuration_ instead — a plain C# class with a property named `Id` is automatically treated as the primary key, no annotations required. You only reach for annotations or fluent config (which we'll do in 2.4) when you need to override the convention.

Create a `Data` folder in the API project, then `Document.cs`:

```csharp
namespace Cortex.Api.Data;

public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string FileName { get; set; }
    public required string ContentHash { get; set; }   // SHA-256 of file bytes — enables idempotent re-upload checks in Phase 3
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<DocumentChunk> Chunks { get; set; } = [];
}
```

And `DocumentChunk.cs`:

```csharp
using Pgvector;

namespace Cortex.Api.Data;

public class DocumentChunk
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid DocumentId { get; set; }
    public Document? Document { get; set; }

    public required string Content { get; set; }
    public required int ChunkIndex { get; set; }   // preserves original order within the source document

    public required Vector Embedding { get; set; }   // from Pgvector.EntityFrameworkCore
}
```

Two C# things worth calling out since they don't exist in Java:

- `required` (on a property) — the compiler forces every caller to set that property when constructing the object. It's a stricter, compile-time version of what you'd enforce with a constructor + validation in Java.
- `= []` — target-typed collection literal (C# 12), shorthand for `= new List<DocumentChunk>()`.

**Decision point you need to make now, not later:** the `Embedding` column's dimensionality depends entirely on which embedding model you pick in Phase 3. `nomic-embed-text` via Ollama produces 768-dimensional vectors; OpenAI's `text-embedding-3-small` produces 1536. This number gets hardcoded into the database schema in the next step, so decide now — I'd suggest starting with Ollama + `nomic-embed-text` (free, offline, 768 dims) since it removes API key friction from your dev loop entirely.

**Commit:** `feat: define Document and DocumentChunk entity models`

---

## 2.4 — Create the DbContext

**Concept first.** `DbContext` is roughly your JPA `EntityManager` and Hibernate `Session` combined — it tracks entity state, translates LINQ queries to SQL, and manages the unit-of-work/change-tracking that eventually becomes an `UPDATE`/`INSERT` on `SaveChanges()`.

```csharp
using Microsoft.EntityFrameworkCore;

namespace Cortex.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentChunk> Chunks => Set<DocumentChunk>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector"); // generates `CREATE EXTENSION IF NOT EXISTS vector` in the migration

        modelBuilder.Entity<DocumentChunk>()
            .Property(c => c.Embedding)
            .HasColumnType("vector(768)"); // matches nomic-embed-text's output dimension — change if you picked a different model

        modelBuilder.Entity<DocumentChunk>()
            .HasOne(c => c.Document)
            .WithMany(d => d.Chunks)
            .HasForeignKey(c => c.DocumentId)
            .OnDelete(DeleteBehavior.Cascade); // deleting a Document deletes its Chunks — no orphaned rows

        base.OnModelCreating(modelBuilder);
    }
}
```

Notice the class declaration: `ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)` — that's a **primary constructor** (C# 12), a compact way of writing what used to require an explicit constructor body just to call `base(options)`. Purely modern-C# syntax sugar; functionally identical to the old verbose form.

You do **not** register this with `builder.Services.AddDbContext<>()` yourself — that already happened via `AddNpgsqlDbContext<ApplicationDbContext>(...)` in step 2.2. That's an Aspire-specific shortcut that most generic EF Core tutorials online won't show you, because they're not written with Aspire's service-discovery model in mind.

**Commit:** `feat: add ApplicationDbContext`

---

## 2.5 — Create and Apply the First Migration

**Concept first.** EF Core Migrations are the code-first equivalent of Flyway/Liquibase: you change your C# model, run a tool, and it diffs your model against its last known snapshot to generate a versioned SQL script (`Up()`/`Down()` methods in a generated C# file, not raw `.sql`, though raw SQL is under the hood).

**Install the EF Core CLI tool** (this is a `dotnet` global tool, separate from the NuGet packages you added earlier — think of it like installing a Maven plugin globally versus per-project):

```bash
dotnet tool install --global dotnet-ef
dotnet ef --version   # confirm it's on your PATH
```

**Here's the wrinkle specific to Aspire projects.** `dotnet ef migrations add` needs to _construct_ your `DbContext` at design time to inspect its model — but your real connection string only exists once the AppHost is running and injecting it via environment variables. EF's tooling doesn't know anything about Aspire's runtime service discovery. The clean fix is a small factory that gives the tooling a throwaway, hardcoded connection string used _only_ for generating migrations — never for running the actual app:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cortex.Api.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=cortexdb;Username=postgres;Password=postgres",
            o => o.UseVector());
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
```

(The port/credentials here should match whatever Aspire's Postgres resource actually uses locally — with `WithHostPort(5432)` from step 2.2, port 5432 is predictable. Default Aspire Postgres credentials are typically `postgres`/a generated password unless you pinned one — check the dashboard's env vars for the real password if this doesn't connect.)

**Generate the migration:**

```bash
cd src/Cortex.Api
dotnet ef migrations add InitialCreate
```

This creates a `Migrations/` folder with a timestamped file containing `Up()` (create tables, add the vector column, create the extension) and `Down()` (undo it). Open it and skim it — you should recognize your two entities as `CreateTable` calls, and see the `vector(768)` column type you configured.

**Apply it.** For Aspire projects, the cleanest approach is applying migrations _programmatically at startup_ rather than via CLI — it sidesteps the design-time connection-string mismatch entirely, since at runtime the real Aspire-injected connection string is already available. In `Cortex.Api/Program.cs`, after `var app = builder.Build();`:

```csharp
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}
```

**Checkpoint — verify the schema landed:**

```bash
docker exec -it <postgres-container-name> psql -U postgres -d cortexdb -c "\dt"
```

(Find the container name via `docker ps` or the Aspire dashboard's resource details.) You should see `Documents` and `Chunks` tables. Then:

```bash
docker exec -it <postgres-container-name> psql -U postgres -d cortexdb -c "\d \"Chunks\""
```

Confirm the `Embedding` column shows type `vector(768)`. If you added `WithPgAdmin()` in 2.2, you can do this same inspection visually in the browser instead of the CLI.

**Commit:** `feat: add initial database migration`

---

## Common Errors You'll Likely Hit

- **`relation "vector" does not exist` / extension errors** — the `pgvector/pgvector` image wasn't actually used (double-check `.WithImage(...)` in AppHost), or `HasPostgresExtension("vector")` is missing from `OnModelCreating`.
- **`dotnet ef` can't find the DbContext** — usually means the design-time factory isn't being discovered; confirm it implements `IDesignTimeDbContextFactory<ApplicationDbContext>` exactly and lives in the same project you're running the command from.
- **Migration applies but connecting manually with `psql` fails** — password mismatch between what you hardcoded in the design-time factory and what Aspire actually generated. Check the real value in the dashboard's env vars for the `postgres` resource.
- **Port conflict on 5432** — if you already have a local Postgres running outside Docker, either stop it or change `WithHostPort` to something else (e.g. `5433`) and update the design-time factory to match.

---

**Checkpoint before moving to Phase 3:** you should be able to run the AppHost, see all resources green in the dashboard, and see two empty but correctly-shaped tables in the database. That's the whole of Phase 2 — nothing here writes or reads real data yet; that starts in Phase 3 (ingestion).

Run through this and let me know where it breaks — that's normal, and debugging the first real error is usually where the actual learning happens.

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

- [ ] Add NuGet packages: `Npgsql.EntityFrameworkCore.PostgreSQL`, `Pgvector.EntityFrameworkCore`

  **Commit:** `chore: add EF Core and pgvector NuGet packages`

- [ ] In AppHost, add a Postgres container resource (pgvector-enabled image) and wire it to the API project — Aspire injects the connection string automatically, no manual "wait for DB" scripts needed.

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

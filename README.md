# AI-Powered-Knowledge-Assistant

A internal-docs / notes assistant where users upload documents and chat with an AI that answers using retrieval-augmented generation (RAG) over their own content.

Tech Stack

- .NET 10 (current LTS) with C# 13/14 features
- Blazor Web App (the unified render mode — server + WASM in one project)
- Microsoft.Extensions.AI for a provider-agnostic AI abstraction layer
- Semantic Kernel for orchestration (or Microsoft.Extensions.VectorData for a lighter-weight approach)
- EF Core 9/10 with a vector-capable store (Postgres + pgvector, or Azure AI Search)
- .NET Aspire for local orchestration (API + DB + vector store as one dashboard)
- Native AOT for the API project (optional stretch goal, shows perf awareness)
- Auth via ASP.NET Core Identity or Entra ID

Core Features

1. Upload documents (PDF/Markdown) → chunk → embed → store vectors
2. Chat interface with streaming responses (SignalR or Blazor's built-in streaming rendering)
3. RAG pipeline: retrieve relevant chunks, ground the AI's answer, show citations
4. Conversation history persisted per user
5. Swap-able model provider (demonstrate abstraction: OpenAI, Azure OpenAI, or a local model via Ollama)

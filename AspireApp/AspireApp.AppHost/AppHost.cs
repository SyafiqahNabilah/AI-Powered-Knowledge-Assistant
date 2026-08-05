var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("postgres")
    .WithImage("pgvector/pgvector", "pg16")   // postgres 16 + pgvector extension baked in
    .WithDataVolume()                          // persist data across `dotnet run` restarts
    .WithHostPort(5432)                        // pin the port so you can psql/pgAdmin in manually
    .WithPgAdmin();                             // optional: web UI to poke at the DB

var cortexDb = postgres.AddDatabase("cortexdb");

var apiService = builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(cortexDb)
    .WaitFor(cortexDb); //wait for the database to be ready before starting the API service

var gateway = builder.AddBlazorGateway("gateway")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithReference(gateway)
    .WaitFor(gateway);

builder.Build().Run();

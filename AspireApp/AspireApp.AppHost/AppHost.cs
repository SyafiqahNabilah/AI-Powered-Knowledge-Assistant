var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

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

using ApiBuilder.Application.Abstractions;

namespace ApiBuilder.Infrastructure.Providers;

public class StubAiProvider : IAiProvider
{
    public Task<string> GenerateOpenApiYamlAsync(string brief, CancellationToken ct = default)
        => Task.FromResult("openapi: 3.0.3\ninfo:\n  title: generated-from-brief\n  version: 0.1.0\n");
}

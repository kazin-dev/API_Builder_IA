namespace ApiBuilder.Application.Abstractions;

public interface IAiProvider
{
    Task<string> GenerateOpenApiYamlAsync(string brief, CancellationToken ct = default);
}

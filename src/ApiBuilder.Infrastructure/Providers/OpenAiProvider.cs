using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ApiBuilder.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace ApiBuilder.Infrastructure.Providers;

public class OpenAiProvider : IAiProvider
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly string _apiKey;

    public OpenAiProvider(IConfiguration config, IHttpClientFactory httpFactory)
    {
        _apiKey = config["OPENAI_API_KEY"] ?? throw new Exception("OPENAI_API_KEY não configurada");
        _httpFactory = httpFactory;
    }

    public async Task<string> GenerateOpenApiYamlAsync(string brief, CancellationToken ct = default)
    {
        var http = _httpFactory.CreateClient("openai");
        http.BaseAddress = new Uri("https://api.openai.com/v1/");
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var payload = new
        {
            model = "gpt-4o-mini",
            messages = new object[]
            {
                new { role = "system", content = "Você é um gerador de especificações OpenAPI 3.0. Responda SOMENTE com YAML válido, sem explicações, sem markdown." },
                new { role = "user",   content = $"Gere uma especificação OpenAPI 3.0 em YAML para o seguinte sistema:\n{brief}" }
            },
            temperature = 0.2
        };

        using var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        using var resp = await http.PostAsync("chat/completions", content, ct);

        // Tratamento explícito de 429 com mensagem clara
        if (resp.StatusCode == HttpStatusCode.TooManyRequests)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            throw new AiRateLimitException($"OpenAI retornou 429 (rate limit). Detalhes: {body}");
        }

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            throw new AiProviderException($"Falha na chamada OpenAI ({(int)resp.StatusCode}): {body}");
        }

        using var stream = await resp.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

        var yaml = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return (yaml ?? string.Empty).Trim();
    }
}

// Exceções específicas para controller/endpoint poder reagir melhor
public class AiRateLimitException : Exception { public AiRateLimitException(string m) : base(m) { } }
public class AiProviderException : Exception { public AiProviderException(string m) : base(m) { } }

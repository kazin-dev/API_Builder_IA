using ApiBuilder.Application.Abstractions;
using ApiBuilder.Application.UseCases.CreateProject;
using ApiBuilder.Infrastructure.Data;
using ApiBuilder.Infrastructure.Providers; // AiRateLimitException / AiProviderException
using ApiBuilder.Infrastructure.Repositories;
using ApiBuilder.Shared.Dtos;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Serilog;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Serilog básico no console
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Host.UseSerilog();

// DbContext (SQL Server LocalDB em dev)
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Sql")));

// HttpClient nomeado "openai" com retry exponencial (429 e 5xx)
var delays = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMilliseconds(250), retryCount: 5);
builder.Services.AddHttpClient("openai")
    .AddPolicyHandler(
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests || (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(delays)
    );

// DI
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IAiProvider, OpenAiProvider>(); // IA real
builder.Services.AddValidatorsFromAssemblyContaining<CreateProjectValidator>();
builder.Services.AddScoped<CreateProjectHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (liberado no dev)
builder.Services.AddCors(o =>
    o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseCors();

// POST /projects — valida entrada, chama handler e trata erros do provider
app.MapPost("/projects", async (
    CreateProjectHandler handler,
    CreateProjectCommand cmd,
    IValidator<CreateProjectCommand> validator,
    CancellationToken ct) =>
{
    var result = validator.Validate(cmd);
    if (!result.IsValid)
        return Results.BadRequest(result.Errors.Select(e => e.ErrorMessage));

    try
    {
        var id = await handler.Handle(cmd, ct);
        return Results.Created($"/projects/{id}", new { id });
    }
    catch (AiRateLimitException ex)
    {
        return Results.Problem(
            title: "Limite de requisições atingido na OpenAI",
            detail: ex.Message,
            statusCode: 429);
    }
    catch (AiProviderException ex)
    {
        return Results.Problem(
            title: "Falha ao gerar OpenAPI via IA",
            detail: ex.Message,
            statusCode: 502);
    }
});

// GET /projects/{id}
app.MapGet("/projects/{id:guid}", async (IProjectRepository repo, Guid id, CancellationToken ct) =>
{
    var p = await repo.GetAsync(id, ct);
    if (p is null)
        return Results.NotFound();

    return Results.Ok(new ProjectDto(p.Id, p.Title, p.OpenApiYaml));
});

app.Run();

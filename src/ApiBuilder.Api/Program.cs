using ApiBuilder.Application.Abstractions;
using ApiBuilder.Application.UseCases.CreateProject;
using ApiBuilder.Infrastructure.Data;
using ApiBuilder.Infrastructure.Providers;
using ApiBuilder.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog básico no console
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Host.UseSerilog();

// DbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Sql")));

// DI
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IAiProvider, StubAiProvider>();
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

// Endpoints mínimos
app.MapPost("/projects", async (CreateProjectHandler handler, CreateProjectCommand cmd, CancellationToken ct) =>
{
    var id = await handler.Handle(cmd, ct);
    return Results.Created($"/projects/{id}", new { id });
});

app.MapGet("/projects/{id:guid}", async (IProjectRepository repo, Guid id, CancellationToken ct) =>
{
    var p = await repo.GetAsync(id, ct);
    return p is null ? Results.NotFound() : Results.Ok(new { p.Id, p.Title, p.OpenApiYaml });
});

app.Run();

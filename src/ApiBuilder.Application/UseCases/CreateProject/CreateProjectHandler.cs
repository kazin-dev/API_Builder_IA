using ApiBuilder.Application.Abstractions;
using ApiBuilder.Domain;

namespace ApiBuilder.Application.UseCases.CreateProject;

public class CreateProjectHandler
{
    private readonly IAiProvider _ai;
    private readonly IProjectRepository _repo;

    public CreateProjectHandler(IAiProvider ai, IProjectRepository repo)
    {
        _ai = ai;
        _repo = repo;
    }

    public async Task<Guid> Handle(CreateProjectCommand cmd, CancellationToken ct = default)
    {
        var project = new Project(cmd.Title, cmd.Brief);

        // 🔁 antes (stub): project.AttachOpenApi("openapi: 3.0.3\ninfo:\n  title: stub\n  version: 0.1.0\n");
        // ✅ agora (IA real):
        var yaml = await _ai.GenerateOpenApiYamlAsync(cmd.Brief, ct);
        project.AttachOpenApi(yaml);

        await _repo.AddAsync(project, ct);
        await _repo.SaveChangesAsync(ct);
        return project.Id;
    }
}

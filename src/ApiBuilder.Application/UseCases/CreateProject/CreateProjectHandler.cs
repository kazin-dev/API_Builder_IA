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

        // Dia 1: stub; no Dia 2 chamaremos a IA de verdade
        project.AttachOpenApi("openapi: 3.0.3\ninfo:\n  title: stub\n  version: 0.1.0\n");

        await _repo.AddAsync(project, ct);
        await _repo.SaveChangesAsync(ct);
        return project.Id;
    }
}

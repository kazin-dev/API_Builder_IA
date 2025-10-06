using ApiBuilder.Application.Abstractions;
using ApiBuilder.Domain;
using ApiBuilder.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiBuilder.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _db;
    public ProjectRepository(AppDbContext db) => _db = db;

    public Task<Project?> GetAsync(Guid id, CancellationToken ct = default) =>
        _db.Projects.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AddAsync(Project project, CancellationToken ct = default) =>
        await _db.Projects.AddAsync(project, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);
}

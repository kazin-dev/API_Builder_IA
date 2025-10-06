using ApiBuilder.Domain;

namespace ApiBuilder.Application.Abstractions;

public interface IProjectRepository
{
    Task AddAsync(Project project, CancellationToken ct = default);
    Task<Project?> GetAsync(Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

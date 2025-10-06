using ApiBuilder.Domain;
using Microsoft.EntityFrameworkCore;

namespace ApiBuilder.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Project> Projects => Set<Project>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Project>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Brief).IsRequired();
            e.Property(x => x.OpenApiYaml);
        });
    }
}

namespace ApiBuilder.Domain;

public class Project
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = null!;
    public string Brief { get; private set; } = null!;
    public string? OpenApiYaml { get; private set; }

    private Project() { }

    public Project(string title, string brief)
    {
        Title = title;
        Brief = brief;
    }

    public void AttachOpenApi(string yaml) => OpenApiYaml = yaml;
}

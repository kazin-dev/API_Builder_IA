using FluentValidation;

namespace ApiBuilder.Application.UseCases.CreateProject;

public class CreateProjectValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(200).WithMessage("Título muito longo.");

        RuleFor(x => x.Brief)
            .NotEmpty().WithMessage("A descrição (brief) é obrigatória.")
            .MinimumLength(10).WithMessage("A descrição é muito curta.");
    }
}

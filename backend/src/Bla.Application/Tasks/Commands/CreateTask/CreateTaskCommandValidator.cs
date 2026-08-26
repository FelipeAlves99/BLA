using FluentValidation;

namespace Bla.Application.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(command => command.Title).NotEmpty().MaximumLength(120);
        RuleFor(command => command.Description).MaximumLength(2000).When(command => command.Description is not null);
        RuleFor(command => command.Status).IsInEnum();
    }
}

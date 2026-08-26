using FluentValidation;

namespace Bla.Application.Tasks.Commands.UpdateTask;

public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.Title).NotEmpty().MaximumLength(120);
        RuleFor(command => command.Description).MaximumLength(2000).When(command => command.Description is not null);
        RuleFor(command => command.Status).IsInEnum();
    }
}

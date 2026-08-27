using FluentValidation;

namespace Bla.Application.Users.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(command => command.Username).NotEmpty().MinimumLength(3).MaximumLength(50).Matches("^[a-zA-Z0-9._-]+$");
        RuleFor(command => command.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(command => command.DisplayName).NotEmpty().MaximumLength(200);
        RuleFor(command => command.Password).MinimumLength(12).MaximumLength(128);
    }
}

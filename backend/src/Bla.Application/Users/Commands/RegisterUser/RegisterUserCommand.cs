using Bla.Application.Common.Interfaces;
using Bla.Domain.Common;
using MediatR;

namespace Bla.Application.Users.Commands.RegisterUser;

public sealed record RegisterUserCommand(string Username, string Email, string DisplayName, string Password) : IRequest<Result<IdentityRegistrationResult>>;

using Bla.Application.Common.Interfaces;
using Bla.Domain.Common;
using Bla.Domain.Identity;
using MediatR;

namespace Bla.Application.Users.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler(IAppDbContext db, IIdentityAdministration identityAdministration) : IRequestHandler<RegisterUserCommand, Result<IdentityRegistrationResult>>
{
    public async Task<Result<IdentityRegistrationResult>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var registration = await identityAdministration.RegisterAsync(request.Username.Trim(), request.Email.Trim(), request.DisplayName.Trim(), request.Password, ct);
        if (!registration.IsSuccess) return Result.Success(registration);

        await db.Users.AddAsync(new ApplicationUser(registration.UserId!.Value, request.Email.Trim(), request.DisplayName.Trim()), ct);
        await db.SaveChangesAsync(ct);
        return Result.Success(registration);
    }
}

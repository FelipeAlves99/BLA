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
        var userId = registration.UserId!.Value;

        try
        {
            await db.Users.AddAsync(new ApplicationUser(userId, request.Email.Trim(), request.DisplayName.Trim()), ct);
            await db.SaveChangesAsync(ct);
            return Result.Success(registration);
        }
        catch (Exception persistenceException)
        {
            try
            {
                await identityAdministration.DeleteAsync(userId, ct);
            }
            catch (Exception compensationException)
            {
                throw new AggregateException("Local user persistence and Keycloak compensation both failed.", persistenceException, compensationException);
            }

            throw;
        }
    }
}

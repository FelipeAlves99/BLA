namespace Bla.Application.Common.Interfaces;

public interface IIdentityAdministration
{
    Task<IdentityRegistrationResult> RegisterAsync(string username, string email, string displayName, string password, CancellationToken ct);
    Task DeleteAsync(Guid userId, CancellationToken ct);
}

public sealed record IdentityRegistrationResult(Guid? UserId, bool IsConflict, string? Error)
{
    public bool IsSuccess => UserId is not null && !IsConflict && Error is null;
}

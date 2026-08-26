using System.Security.Claims;
using Bla.Application.Common.Interfaces;

namespace Bla.Infrastructure.Identity;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid Id => Guid.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? accessor.HttpContext?.User.FindFirstValue("sub"), out var id) ? id : throw new UnauthorizedAccessException("JWT subject must be a UUID.");
    public string? Email => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email) ?? accessor.HttpContext?.User.FindFirstValue("email");
    public string? DisplayName => accessor.HttpContext?.User.Identity?.Name ?? accessor.HttpContext?.User.FindFirstValue("preferred_username");
}

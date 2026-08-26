namespace Bla.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid Id { get; }
    string? Email { get; }
    string? DisplayName { get; }
}

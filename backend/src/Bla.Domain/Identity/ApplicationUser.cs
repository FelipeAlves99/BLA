namespace Bla.Domain.Identity;

public sealed class ApplicationUser
{
    private ApplicationUser() { }
    public ApplicationUser(Guid id, string? email, string? displayName)
    {
        Id = id;
        Email = email;
        DisplayName = displayName;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public string? Email { get; private set; }
    public string? DisplayName { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
}

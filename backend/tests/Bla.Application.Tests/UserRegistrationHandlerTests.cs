using Bla.Application.Common.Interfaces;
using Bla.Application.Users.Commands.RegisterUser;
using Bla.Domain.Identity;
using Bla.Domain.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Bla.Application.Tests;

public sealed class UserRegistrationHandlerTests
{
    [Fact]
    public async Task RegisterUserCommand_IdentityProviderCreatesUser_PersistsLocalUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        await using var db = new TestDbContext();
        var handler = new RegisterUserCommandHandler(db, new TestIdentityAdministration(new IdentityRegistrationResult(userId, false, null)));

        // Act
        var result = await handler.Handle(new RegisterUserCommand("new.user", "new.user@bla.local", "New User", "a-strong-password"), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.UserId.ShouldBe(userId);
        db.Users.Single().Id.ShouldBe(userId);
    }

    [Fact]
    public async Task RegisterUserCommand_IdentityProviderReportsConflict_DoesNotPersistLocalUser()
    {
        // Arrange
        await using var db = new TestDbContext();
        var handler = new RegisterUserCommandHandler(db, new TestIdentityAdministration(new IdentityRegistrationResult(null, true, null)));

        // Act
        var result = await handler.Handle(new RegisterUserCommand("existing", "existing@bla.local", "Existing User", "a-strong-password"), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.IsConflict.ShouldBeTrue();
        db.Users.ShouldBeEmpty();
    }

    private sealed class TestIdentityAdministration(IdentityRegistrationResult result) : IIdentityAdministration
    {
        public Task<IdentityRegistrationResult> RegisterAsync(string username, string email, string displayName, string password, CancellationToken ct) => Task.FromResult(result);
    }

    private sealed class TestDbContext : DbContext, IAppDbContext
    {
        public TestDbContext() : base(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options) { }
        public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
    }
}

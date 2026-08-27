using Bla.Application.Common.Interfaces;
using Bla.Application.Tasks.Queries.GetTask;
using Bla.Domain.Identity;
using Bla.Domain.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Bla.Application.Tests;

public sealed class TaskOwnershipTests
{
    [Fact]
    public async Task GetTaskQuery_TaskOwnedByDifferentUser_ReturnsNotFound()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var callerId = Guid.NewGuid();
        await using var db = new TestDbContext();
        await db.Users.AddAsync(new ApplicationUser(ownerId, null, null));
        var task = new TaskItem(ownerId, "Private", null, TaskStatus.Todo, null);
        await db.Tasks.AddAsync(task);
        await db.SaveChangesAsync();
        var handler = new GetTaskQueryHandler(db, new TestCurrentUser(callerId));

        // Act
        var result = await handler.Handle(new GetTaskQuery(task.Id), CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Task was not found.");
    }

    private sealed class TestCurrentUser(Guid id) : ICurrentUser
    {
        public Guid Id => id;
        public string? Email => null;
        public string? DisplayName => null;
    }

    private sealed class TestDbContext : DbContext, IAppDbContext
    {
        public TestDbContext() : base(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options) { }
        public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
    }
}

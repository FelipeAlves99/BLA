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
    public async Task Get_returns_not_found_when_the_task_is_owned_by_another_user()
    {
        var owner = Guid.NewGuid(); var caller = Guid.NewGuid();
        await using var db = new TestDbContext();
        await db.Users.AddAsync(new ApplicationUser(owner, null, null));
        var task = new TaskItem(owner, "Private", null, TaskStatus.Todo, null);
        await db.Tasks.AddAsync(task); await db.SaveChangesAsync();
        var result = await new GetTaskQueryHandler(db, new TestCurrentUser(caller)).Handle(new GetTaskQuery(task.Id), CancellationToken.None);
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Task was not found.");
    }
    private sealed class TestCurrentUser(Guid id) : ICurrentUser { public Guid Id => id; public string? Email => null; public string? DisplayName => null; }
    private sealed class TestDbContext : DbContext, IAppDbContext
    {
        public TestDbContext() : base(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options) { }
        public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
    }
}

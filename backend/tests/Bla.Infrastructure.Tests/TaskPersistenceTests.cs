using Bla.Application.Common.Interfaces;
using Bla.Domain.Identity;
using Bla.Domain.Tasks;
using Bla.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Shouldly;
using TaskItemStatus = Bla.Domain.Tasks.TaskStatus;

namespace Bla.Infrastructure.Tests;

public sealed class TaskPersistenceTests
{
    [Fact]
    public async Task Tasks_GlobalOwnerFilter_ReturnsOnlyCurrentUsersTasks()
    {
        // Arrange
        var currentUser = new TestCurrentUser(Guid.NewGuid());
        await using var db = CreateDb(currentUser);
        var otherUser = Guid.NewGuid();
        await db.Users.AddRangeAsync(new ApplicationUser(currentUser.Id, null, null), new ApplicationUser(otherUser, null, null));
        await db.Tasks.AddRangeAsync(new TaskItem(currentUser.Id, "Mine", null, TaskItemStatus.Todo, null), new TaskItem(otherUser, "Other", null, TaskItemStatus.Todo, null));
        await db.SaveChangesAsync();

        // Act
        var tasks = await db.Tasks.ToListAsync();

        // Assert
        tasks.Count.ShouldBe(1);
        tasks.Single().OwnerId.ShouldBe(currentUser.Id);
    }

    [Fact]
    public async Task SeedAsync_DemoUserHasNoTasks_AddsDemoTasksOnlyOnce()
    {
        // Arrange
        var services = new ServiceCollection();
        var currentUser = new TestCurrentUser(Guid.NewGuid());
        var databaseName = Guid.NewGuid().ToString();
        services.AddSingleton<ICurrentUser>(currentUser);
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(databaseName));
        await using var provider = services.BuildServiceProvider();
        var environment = new TestHostEnvironment();

        // Act
        await provider.SeedAsync(environment);
        await provider.SeedAsync(environment);
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var demoUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var tasks = await db.Tasks.IgnoreQueryFilters().Where(task => task.OwnerId == demoUserId).ToListAsync();

        // Assert
        tasks.Count.ShouldBe(3);
    }

    private static AppDbContext CreateDb(ICurrentUser currentUser) => new(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options, currentUser);

    private sealed class TestCurrentUser(Guid id) : ICurrentUser
    {
        public Guid Id => id;
        public string? Email => null;
        public string? DisplayName => null;
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "Bla.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}

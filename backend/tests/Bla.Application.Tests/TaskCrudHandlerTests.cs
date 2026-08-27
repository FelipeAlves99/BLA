using Bla.Application.Common.Interfaces;
using Bla.Application.Tasks.Commands.CreateTask;
using Bla.Application.Tasks.Commands.DeleteTask;
using Bla.Application.Tasks.Commands.UpdateTask;
using Bla.Application.Tasks.Queries.GetTask;
using Bla.Application.Tasks.Queries.ListTasks;
using Bla.Domain.Identity;
using Bla.Domain.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Bla.Application.Tests;

public sealed class TaskCrudHandlerTests
{
    [Fact]
    public async Task CreateTaskCommand_ValidTask_CreatesCurrentUserAndOwnedTask()
    {
        // Arrange
        var user = new TestCurrentUser(Guid.NewGuid());
        await using var db = new TestDbContext();
        var handler = new CreateTaskCommandHandler(db, user);
        var command = new CreateTaskCommand("Write tests", "Cover CRUD", TaskStatus.Todo, new DateOnly(2026, 9, 1));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        db.Users.Single().Id.ShouldBe(user.Id);
        var task = db.Tasks.Single();
        task.Id.ShouldBe(result.Value);
        task.OwnerId.ShouldBe(user.Id);
        task.Title.ShouldBe("Write tests");
    }

    [Fact]
    public async Task GetTaskQuery_CurrentUserOwnsTask_ReturnsTask()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        await using var db = new TestDbContext();
        var task = await AddTaskForUserAsync(db, ownerId, "Mine");
        var handler = new GetTaskQueryHandler(db, new TestCurrentUser(ownerId));

        // Act
        var result = await handler.Handle(new GetTaskQuery(task.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(task.Id);
        result.Value.Title.ShouldBe("Mine");
    }

    [Fact]
    public async Task ListTasksQuery_CurrentUserOwnsOneOfTwoTasks_ReturnsOnlyOwnedTask()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        await using var db = new TestDbContext();
        var ownedTask = await AddTaskForUserAsync(db, ownerId, "Mine");
        await AddTaskForUserAsync(db, Guid.NewGuid(), "Not mine");
        var handler = new ListTasksQueryHandler(db, new TestCurrentUser(ownerId));

        // Act
        var result = await handler.Handle(new ListTasksQuery(), CancellationToken.None);

        // Assert
        result.Count.ShouldBe(1);
        result.Single().Id.ShouldBe(ownedTask.Id);
    }

    [Fact]
    public async Task UpdateTaskCommand_CurrentUserOwnsTask_UpdatesTask()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        await using var db = new TestDbContext();
        var task = await AddTaskForUserAsync(db, ownerId, "Before");
        var handler = new UpdateTaskCommandHandler(db, new TestCurrentUser(ownerId));
        var command = new UpdateTaskCommand(task.Id, "After", "Updated", TaskStatus.Done, new DateOnly(2026, 10, 1));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var updatedTask = await db.Tasks.SingleAsync();
        updatedTask.Title.ShouldBe("After");
        updatedTask.Status.ShouldBe(TaskStatus.Done);
    }

    [Fact]
    public async Task UpdateTaskCommand_CompletedTask_UpdatesOnlyDescription()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        await using var db = new TestDbContext();
        var task = await AddTaskForUserAsync(db, ownerId, "Completed task");
        task.Update("Completed task", "Before", TaskStatus.Done, new DateOnly(2026, 8, 26));
        await db.SaveChangesAsync();
        var handler = new UpdateTaskCommandHandler(db, new TestCurrentUser(ownerId));
        var command = new UpdateTaskCommand(task.Id, "Changed title", "Updated description", TaskStatus.Todo, new DateOnly(2026, 9, 1));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var updatedTask = await db.Tasks.SingleAsync();
        updatedTask.Title.ShouldBe("Completed task");
        updatedTask.Description.ShouldBe("Updated description");
        updatedTask.Status.ShouldBe(TaskStatus.Done);
        updatedTask.DueDate.ShouldBe(new DateOnly(2026, 8, 26));
    }

    [Fact]
    public async Task DeleteTaskCommand_CurrentUserOwnsTask_DeletesTask()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        await using var db = new TestDbContext();
        var task = await AddTaskForUserAsync(db, ownerId, "Delete me");
        var handler = new DeleteTaskCommandHandler(db, new TestCurrentUser(ownerId));

        // Act
        var result = await handler.Handle(new DeleteTaskCommand(task.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        (await db.Tasks.CountAsync()).ShouldBe(0);
    }

    private static async Task<TaskItem> AddTaskForUserAsync(TestDbContext db, Guid userId, string title)
    {
        await db.Users.AddAsync(new ApplicationUser(userId, null, null));
        var task = new TaskItem(userId, title, null, TaskStatus.Todo, null);
        await db.Tasks.AddAsync(task);
        await db.SaveChangesAsync();
        return task;
    }

    private sealed class TestCurrentUser(Guid id) : ICurrentUser
    {
        public Guid Id => id;
        public string? Email => "test@bla.local";
        public string? DisplayName => "Test";
    }

    private sealed class TestDbContext : DbContext, IAppDbContext
    {
        public TestDbContext() : base(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options) { }
        public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
    }
}

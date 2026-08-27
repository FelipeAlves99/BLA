using Bla.Domain.Identity;
using Bla.Domain.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskItemStatus = Bla.Domain.Tasks.TaskStatus;

namespace Bla.Infrastructure.Persistence;

public static class DevelopmentDataSeeder
{
    private static readonly Guid DemoUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static async Task SeedAsync(this IServiceProvider services, IHostEnvironment environment, CancellationToken ct = default)
    {
        if (!environment.IsDevelopment()) return;

        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (await db.Tasks.IgnoreQueryFilters().AnyAsync(task => task.OwnerId == DemoUserId, ct)) return;

        if (await db.Users.FindAsync([DemoUserId], ct) is null)
            await db.Users.AddAsync(new ApplicationUser(DemoUserId, "demo@bla.local", "Demo User"), ct);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        await db.Tasks.AddRangeAsync([
            new TaskItem(DemoUserId, "Review today's priorities", "Confirm the work planned for today.", TaskItemStatus.InProgress, today),
            new TaskItem(DemoUserId, "Prepare the presentation", "Summarize the architecture and test coverage.", TaskItemStatus.Todo, today.AddDays(1)),
            new TaskItem(DemoUserId, "Welcome to BLA", "This completed task demonstrates the completed state.", TaskItemStatus.Done, today.AddDays(-1)),
        ], ct);
        await db.SaveChangesAsync(ct);
    }
}

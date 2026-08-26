using Bla.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bla.Application.Tasks.Queries.ListTasks;

internal sealed class ListTasksQueryHandler(IAppDbContext db, ICurrentUser currentUser) : IRequestHandler<ListTasksQuery, IReadOnlyList<TaskDto>>
{
    public async Task<IReadOnlyList<TaskDto>> Handle(ListTasksQuery request, CancellationToken ct) =>
        await db.Tasks.AsNoTracking().Where(task => task.OwnerId == currentUser.Id).OrderBy(task => task.DueDate).Select(task => new TaskDto(task.Id, task.Title, task.Description, task.Status, task.DueDate, task.CreatedAtUtc, task.UpdatedAtUtc)).ToListAsync(ct);
}

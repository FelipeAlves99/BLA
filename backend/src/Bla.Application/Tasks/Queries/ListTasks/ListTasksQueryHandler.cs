using Bla.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bla.Application.Tasks.Queries.ListTasks;

internal sealed class ListTasksQueryHandler(
    IAppDbContext db,
    ICurrentUser currentUser) : IRequestHandler<ListTasksQuery, IReadOnlyList<ListTasksResponse>>
{
    public async Task<IReadOnlyList<ListTasksResponse>> Handle(ListTasksQuery request, CancellationToken ct)
    {
        return await db.Tasks
            .AsNoTracking()
            .Where(task => task.OwnerId == currentUser.Id)
            .OrderBy(task => task.CreatedAtUtc)
            .Select(task => new ListTasksResponse(
                task.Id,
                task.Title,
                task.Status,
                task.DueDate))
            .ToListAsync(ct);
    }
}

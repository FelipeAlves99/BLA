using Bla.Application.Common.Interfaces;
using Bla.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bla.Application.Tasks.Queries.GetTask;

internal sealed class GetTaskQueryHandler(
    IAppDbContext db,
    ICurrentUser currentUser) : IRequestHandler<GetTaskQuery, Result<GetTaskResponse>>
{
    public async Task<Result<GetTaskResponse>> Handle(GetTaskQuery request, CancellationToken ct)
    {
        var task = await db.Tasks
            .AsNoTracking()
            .SingleOrDefaultAsync(
                task => task.Id == request.Id && task.OwnerId == currentUser.Id,
                ct);

        return task is null
            ? Result.Failure<GetTaskResponse>("Task was not found.")
            : Result.Success(new GetTaskResponse(
                task.Id,
                task.Title,
                task.Description,
                task.Status,
                task.DueDate,
                task.CreatedAtUtc,
                task.UpdatedAtUtc));
    }
}

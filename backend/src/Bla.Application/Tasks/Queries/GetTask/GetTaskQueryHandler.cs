using Bla.Application.Common.Interfaces;
using Bla.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bla.Application.Tasks.Queries.GetTask;

public sealed class GetTaskQueryHandler(IAppDbContext db, ICurrentUser currentUser) : IRequestHandler<GetTaskQuery, Result<TaskDto>>
{
    public async Task<Result<TaskDto>> Handle(GetTaskQuery request, CancellationToken ct)
    {
        var task = await db.Tasks.AsNoTracking().SingleOrDefaultAsync(task => task.Id == request.Id && task.OwnerId == currentUser.Id, ct);
        return task is null ? Result.Failure<TaskDto>("Task was not found.") : Result.Success(TaskMapper.ToDto(task));
    }
}

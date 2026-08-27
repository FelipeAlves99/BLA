using Bla.Application.Common.Interfaces;
using Bla.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskItemStatus = Bla.Domain.Tasks.TaskStatus;

namespace Bla.Application.Tasks.Commands.UpdateTask;

internal sealed class UpdateTaskCommandHandler(IAppDbContext db, ICurrentUser currentUser) : IRequestHandler<UpdateTaskCommand, Result>
{
    public async Task<Result> Handle(UpdateTaskCommand request, CancellationToken ct)
    {
        var task = await db.Tasks.SingleOrDefaultAsync(task => task.Id == request.Id && task.OwnerId == currentUser.Id, ct);
        if (task is null) return Result.Failure("Task was not found.");
        if (task.Status == TaskItemStatus.Done)
        {
            task.UpdateDescription(request.Description?.Trim());
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
        task.Update(request.Title.Trim(), request.Description?.Trim(), request.Status, request.DueDate);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

using Bla.Application.Common.Interfaces;
using Bla.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bla.Application.Tasks.Commands.UpdateTask;

internal sealed class UpdateTaskCommandHandler(IAppDbContext db, ICurrentUser currentUser) : IRequestHandler<UpdateTaskCommand, Result>
{
    public async Task<Result> Handle(UpdateTaskCommand request, CancellationToken ct)
    {
        var task = await db.Tasks.SingleOrDefaultAsync(task => task.Id == request.Id && task.OwnerId == currentUser.Id, ct);
        if (task is null) return Result.Failure("Task was not found.");
        task.Update(request.Title.Trim(), request.Description?.Trim(), request.Status, request.DueDate);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

using Bla.Application.Common.Interfaces;
using Bla.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bla.Application.Tasks.Commands.DeleteTask;

internal sealed class DeleteTaskCommandHandler(IAppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<DeleteTaskCommand, Result>
{
    public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken ct)
    {
        var task = await db.Tasks.SingleOrDefaultAsync(task => task.Id == request.Id && task.OwnerId == currentUser.Id,
            ct);
        if (task is null)
            return Result.Failure("Task was not found.");

        db.Tasks.Remove(task);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

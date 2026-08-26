using Bla.Application.Common.Interfaces;
using Bla.Domain.Common;
using Bla.Domain.Identity;
using Bla.Domain.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bla.Application.Tasks.Commands.CreateTask;

internal sealed class CreateTaskCommandHandler(IAppDbContext db, ICurrentUser currentUser) : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTaskCommand request, CancellationToken ct)
    {
        if (await db.Users.FindAsync([currentUser.Id], ct) is null)
            await db.Users.AddAsync(new ApplicationUser(currentUser.Id, currentUser.Email, currentUser.DisplayName), ct);
        var task = new TaskItem(currentUser.Id, request.Title.Trim(), request.Description?.Trim(), request.Status, request.DueDate);
        await db.Tasks.AddAsync(task, ct);
        await db.SaveChangesAsync(ct);
        return Result.Success(task.Id);
    }
}

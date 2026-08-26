using Bla.Domain.Common;
using MediatR;
using TaskItemStatus = Bla.Domain.Tasks.TaskStatus;

namespace Bla.Application.Tasks.Commands.UpdateTask;

public sealed record UpdateTaskCommand(Guid Id, string Title, string? Description, TaskItemStatus Status, DateOnly? DueDate) : IRequest<Result>;

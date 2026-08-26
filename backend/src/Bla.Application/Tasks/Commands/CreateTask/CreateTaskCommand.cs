using Bla.Domain.Common;
using MediatR;
using TaskItemStatus = Bla.Domain.Tasks.TaskStatus;

namespace Bla.Application.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(string Title, string? Description, TaskItemStatus Status, DateOnly? DueDate) : IRequest<Result<Guid>>;

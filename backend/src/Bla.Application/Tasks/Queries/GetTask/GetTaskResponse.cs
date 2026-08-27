using TaskItemStatus = Bla.Domain.Tasks.TaskStatus;

namespace Bla.Application.Tasks.Queries.GetTask;

public sealed record GetTaskResponse(
    Guid Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateOnly? DueDate,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

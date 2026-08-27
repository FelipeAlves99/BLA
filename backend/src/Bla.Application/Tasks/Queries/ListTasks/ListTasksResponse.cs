using TaskItemStatus = Bla.Domain.Tasks.TaskStatus;

namespace Bla.Application.Tasks.Queries.ListTasks;

public sealed record ListTasksResponse(
    Guid Id,
    string Title,
    TaskItemStatus Status,
    DateOnly? DueDate);

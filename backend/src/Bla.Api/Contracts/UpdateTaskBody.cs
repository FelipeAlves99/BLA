using TaskItemStatus = Bla.Domain.Tasks.TaskStatus;

namespace Bla.Api.Contracts;

public sealed record UpdateTaskBody(string Title, string? Description, TaskItemStatus Status, DateOnly? DueDate);

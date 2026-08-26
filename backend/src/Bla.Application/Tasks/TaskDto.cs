using TaskItemStatus = Bla.Domain.Tasks.TaskStatus;

namespace Bla.Application.Tasks;

public sealed record TaskDto(Guid Id, string Title, string? Description, TaskItemStatus Status, DateOnly? DueDate, DateTimeOffset CreatedAtUtc, DateTimeOffset UpdatedAtUtc);

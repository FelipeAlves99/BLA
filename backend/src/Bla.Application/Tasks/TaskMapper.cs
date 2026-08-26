using Bla.Domain.Tasks;

namespace Bla.Application.Tasks;

internal static class TaskMapper
{
    public static TaskDto ToDto(TaskItem task) => new(task.Id, task.Title, task.Description, task.Status, task.DueDate, task.CreatedAtUtc, task.UpdatedAtUtc);
}

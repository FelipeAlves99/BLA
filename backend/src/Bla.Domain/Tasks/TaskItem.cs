namespace Bla.Domain.Tasks;

public sealed class TaskItem
{
    private TaskItem() { }
    public TaskItem(Guid ownerId, string title, string? description, TaskStatus status, DateOnly? dueDate)
    {
        Id = Guid.NewGuid(); OwnerId = ownerId; CreatedAtUtc = DateTimeOffset.UtcNow; UpdatedAtUtc = CreatedAtUtc;
        Update(title, description, status, dueDate);
    }
    public Guid Id { get; private set; }
    public Guid OwnerId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public DateOnly? DueDate { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public void Update(string title, string? description, TaskStatus status, DateOnly? dueDate)
    {
        Title = title; Description = description; Status = status; DueDate = dueDate; UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
    public void UpdateDescription(string? description)
    {
        Description = description; UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}

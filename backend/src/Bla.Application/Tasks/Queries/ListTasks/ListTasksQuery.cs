using MediatR;

namespace Bla.Application.Tasks.Queries.ListTasks;

public sealed record ListTasksQuery : IRequest<IReadOnlyList<TaskDto>>;

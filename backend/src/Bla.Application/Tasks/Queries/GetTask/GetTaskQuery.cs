using Bla.Domain.Common;
using MediatR;

namespace Bla.Application.Tasks.Queries.GetTask;

public sealed record GetTaskQuery(Guid Id) : IRequest<Result<GetTaskResponse>>;

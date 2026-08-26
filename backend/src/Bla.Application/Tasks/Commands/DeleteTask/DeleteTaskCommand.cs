using Bla.Domain.Common;
using MediatR;

namespace Bla.Application.Tasks.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid Id) : IRequest<Result>;

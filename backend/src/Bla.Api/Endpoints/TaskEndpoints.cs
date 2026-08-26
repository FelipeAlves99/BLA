using Bla.Api.Common;
using Bla.Api.Common.Extensions;
using Bla.Application.Tasks.Commands.CreateTask;
using Bla.Application.Tasks.Commands.DeleteTask;
using Bla.Application.Tasks.Commands.UpdateTask;
using Bla.Application.Tasks.Queries.GetTask;
using Bla.Application.Tasks.Queries.ListTasks;
using MediatR;
using TaskItemStatus = Bla.Domain.Tasks.TaskStatus;

namespace Bla.Api.Endpoints;

public sealed record UpdateTaskBody(string Title, string? Description, TaskItemStatus Status, DateOnly? DueDate);

public sealed class TaskEndpoints : IEndpointGroup
{
    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/tasks").WithTags("Tasks").RequireAuthorization();
        group.MapGet("/", List).WithName("ListTasks");
        group.MapGet("/{id:guid}", Get).WithName("GetTask").ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPost("/", Create).WithName("CreateTask").Produces(StatusCodes.Status201Created).ProducesProblem(StatusCodes.Status400BadRequest);
        group.MapPut("/{id:guid}", Update).WithName("UpdateTask").Produces(StatusCodes.Status204NoContent).ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status404NotFound);
        group.MapDelete("/{id:guid}", Delete).WithName("DeleteTask").Produces(StatusCodes.Status204NoContent).ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> List(ISender sender, CancellationToken ct) => TypedResults.Ok(await sender.Send(new ListTasksQuery(), ct));
    private static async Task<IResult> Get(Guid id, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetTaskQuery(id), ct);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.ToNotFoundProblem();
    }
    private static async Task<IResult> Create(CreateTaskCommand command, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess ? TypedResults.Created($"/v1/tasks/{result.Value}", new { id = result.Value }) : result.ToProblemDetails();
    }
    private static async Task<IResult> Update(Guid id, UpdateTaskBody body, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateTaskCommand(id, body.Title, body.Description, body.Status, body.DueDate), ct);
        return result.IsSuccess ? TypedResults.NoContent() : result.ToNotFoundProblem();
    }
    private static async Task<IResult> Delete(Guid id, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteTaskCommand(id), ct);
        return result.IsSuccess ? TypedResults.NoContent() : result.ToNotFoundProblem();
    }
}

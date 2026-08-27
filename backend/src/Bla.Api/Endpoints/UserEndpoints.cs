using Bla.Api.Common;
using Bla.Application.Users.Commands.RegisterUser;
using MediatR;

namespace Bla.Api.Endpoints;

public sealed class UserEndpoints : IEndpointGroup
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/v1/users", Register).AllowAnonymous();
    }

    private static async Task<IResult> Register(RegisterUserCommand command, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        if (!result.IsSuccess)
            return TypedResults.Problem(result.Error, statusCode: StatusCodes.Status400BadRequest);

        if (result.Value.IsConflict)
            return TypedResults.Problem("Username or email already in use.", statusCode: StatusCodes.Status409Conflict, title: "Username or email already in use.");

        if (!result.Value.IsSuccess)
            return TypedResults.Problem(result.Value.Error, statusCode: StatusCodes.Status502BadGateway);

        return TypedResults.Created($"/v1/users/{result.Value.UserId}", new { id = result.Value.UserId });
    }
}

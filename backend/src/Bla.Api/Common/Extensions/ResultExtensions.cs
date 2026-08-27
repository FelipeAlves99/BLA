using Bla.Domain.Common;
namespace Bla.Api.Common.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Result result, int statusCode = StatusCodes.Status400BadRequest) => TypedResults.Problem(result.Error, statusCode: statusCode);
    public static IResult ToProblemDetails<T>(this Result<T> result, int statusCode = StatusCodes.Status400BadRequest) => TypedResults.Problem(result.Error, statusCode: statusCode);
    public static IResult ToNotFoundProblem(this Result result) => result.ToProblemDetails(StatusCodes.Status404NotFound);
    public static IResult ToNotFoundProblem<T>(this Result<T> result) => result.ToProblemDetails(StatusCodes.Status404NotFound);
}

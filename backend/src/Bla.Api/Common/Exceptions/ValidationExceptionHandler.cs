using Microsoft.AspNetCore.Diagnostics;
using ValidationException = FluentValidation.ValidationException;

namespace Bla.Api.Common.Exceptions;
public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not ValidationException validation) return false;
        var errors = validation.Errors.GroupBy(error => error.PropertyName).ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new HttpValidationProblemDetails(errors) { Status = StatusCodes.Status400BadRequest, Title = "Validation failed" }, ct);
        return true;
    }
}

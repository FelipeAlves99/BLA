using Bla.Application.Tasks.Commands.CreateTask;
using Shouldly;

namespace Bla.Application.Tests;

public sealed class TaskValidationTests
{
    [Fact]
    public void CreateTaskCommandValidator_BlankTitle_ReturnsValidationError()
    {
        // Arrange
        var validator = new CreateTaskCommandValidator();
        var command = new CreateTaskCommand(" ", null, TaskStatus.Todo, null);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.PropertyName == "Title");
    }
}

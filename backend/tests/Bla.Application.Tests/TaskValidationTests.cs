using Bla.Application.Tasks.Commands.CreateTask;
using Shouldly;
using TaskItemStatus = Bla.Domain.Tasks.TaskStatus;

namespace Bla.Application.Tests;

public sealed class TaskValidationTests
{
    [Fact]
    public void Create_validator_rejects_blank_title()
    {
        var result = new CreateTaskCommandValidator().Validate(new CreateTaskCommand(" ", null, TaskItemStatus.Todo, null));
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.PropertyName == "Title");
    }
}

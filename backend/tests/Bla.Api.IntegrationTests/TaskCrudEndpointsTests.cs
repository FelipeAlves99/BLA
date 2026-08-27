using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Shouldly;

namespace Bla.Api.IntegrationTests;

[Collection(nameof(ApiIntegrationCollection))]
public sealed class TaskCrudEndpointsTests(PostgreSqlFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => fixture.ResetAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task PostTasks_ValidRequest_ReturnsCreatedTask()
    {
        // Arrange
        using var client = fixture.Factory.CreateClient();
        var request = new { title = "Integration task", description = "Created through HTTP", status = "Todo", dueDate = "2026-12-01" };

        // Act
        var response = await client.PostAsJsonAsync("/v1/tasks/", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();
        (await GetTaskIdAsync(response)).ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetTask_OwnerRequestsCreatedTask_ReturnsTask()
    {
        // Arrange
        using var client = fixture.Factory.CreateClient();
        var taskId = await CreateTaskAsync(client, "Read me");

        // Act
        var response = await client.GetAsync($"/v1/tasks/{taskId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("title").GetString().ShouldBe("Read me");
    }

    [Fact]
    public async Task GetTasks_OwnerHasOneTask_ReturnsOwnedTask()
    {
        // Arrange
        using var client = fixture.Factory.CreateClient();
        var taskId = await CreateTaskAsync(client, "List me");

        // Act
        var response = await client.GetAsync("/v1/tasks/");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tasks = await response.Content.ReadFromJsonAsync<JsonElement>();
        tasks.GetArrayLength().ShouldBe(1);
        tasks[0].GetProperty("id").GetGuid().ShouldBe(taskId);
    }

    [Fact]
    public async Task PutTask_OwnerUpdatesTask_ReturnsNoContentAndPersistsUpdate()
    {
        // Arrange
        using var client = fixture.Factory.CreateClient();
        var taskId = await CreateTaskAsync(client, "Before update");
        var request = new { title = "After update", description = "Done", status = "Done", dueDate = "2026-12-02" };

        // Act
        var response = await client.PutAsJsonAsync($"/v1/tasks/{taskId}", request);
        var getResponse = await client.GetAsync($"/v1/tasks/{taskId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var task = await getResponse.Content.ReadFromJsonAsync<JsonElement>();
        task.GetProperty("title").GetString().ShouldBe("After update");
        task.GetProperty("status").GetString().ShouldBe("Done");
    }

    [Fact]
    public async Task DeleteTask_OwnerDeletesTask_ReturnsNoContentAndTaskIsNotFound()
    {
        // Arrange
        using var client = fixture.Factory.CreateClient();
        var taskId = await CreateTaskAsync(client, "Delete me");

        // Act
        var deleteResponse = await client.DeleteAsync($"/v1/tasks/{taskId}");
        var getResponse = await client.GetAsync($"/v1/tasks/{taskId}");

        // Assert
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTask_DifferentUserRequestsOwnerTask_ReturnsNotFound()
    {
        // Arrange
        using var client = fixture.Factory.CreateClient();
        var taskId = await CreateTaskAsync(client, "Private task");
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/v1/tasks/{taskId}");
        request.Headers.Add("X-Test-User", Guid.NewGuid().ToString());

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private static async Task<Guid> CreateTaskAsync(HttpClient client, string title)
    {
        var response = await client.PostAsJsonAsync("/v1/tasks/", new { title, description = (string?)null, status = "Todo", dueDate = (string?)null });
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        return await GetTaskIdAsync(response);
    }

    private static async Task<Guid> GetTaskIdAsync(HttpResponseMessage response) =>
        JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("id").GetGuid();
}

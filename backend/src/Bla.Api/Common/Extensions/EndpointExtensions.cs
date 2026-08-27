namespace Bla.Api.Common.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        foreach (var type in typeof(IEndpointGroup).Assembly.GetTypes().Where(type => type.IsAssignableTo(typeof(IEndpointGroup)) && !type.IsAbstract && !type.IsInterface))
            if (Activator.CreateInstance(type) is IEndpointGroup group) group.Map(app);
        return app;
    }
}

using System.Text.Json.Serialization;
using Asp.Versioning;
using Bla.Api.Common.Exceptions;
using Bla.Api.Common.Extensions;
using Bla.Application;
using Bla.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext());
builder.Services.AddApplication().AddInfrastructure(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Auth:Authority"];
    options.Audience = builder.Configuration["Auth:Audience"];
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
});
builder.Services.AddAuthorization();
if (builder.Environment.IsDevelopment()) builder.Services.AddCors(options => options.AddPolicy("dev", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddApiVersioning(options => { options.DefaultApiVersion = new ApiVersion(1); options.AssumeDefaultVersionWhenUnspecified = true; options.ApiVersionReader = new UrlSegmentApiVersionReader(); });
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks().AddInfrastructureHealthChecks();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();
app.UseExceptionHandler();
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment()) app.UseCors("dev");
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment()) { app.MapOpenApi(); app.MapScalarApiReference(); }
app.MapHealthChecks("/healthz", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/readyz", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });
app.MapGet("/v1/public/ping", () => TypedResults.Ok(new { status = "ok" })).AllowAnonymous();
app.MapEndpoints();
app.Run();
public partial class Program;

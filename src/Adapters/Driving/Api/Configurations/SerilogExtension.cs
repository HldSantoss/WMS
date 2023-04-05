using System.Net;
using System.Security.Claims;
using Elastic.CommonSchema.Serilog;
using Microsoft.AspNetCore.Http.Features;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;

namespace Api.Configurations;

public static class SerilogExtension
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, IConfiguration configuration, string applicationName)
    {
        Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", $"{applicationName} - {Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}")
                    .Enrich.WithCorrelationId()
                    .Enrich.WithExceptionDetails()
                    .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
                    .WriteTo.Async(writeTo => writeTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticsearchSettings:uri"]))
                    {
                        TypeName = null,
                        AutoRegisterTemplate = true,
                        IndexFormat = configuration["ElasticsearchSettings:defaultIndex"],
                        BatchAction = ElasticOpType.Create,
                        CustomFormatter = new EcsTextFormatter(),
                        ModifyConnectionSettings = x => x.BasicAuthentication(configuration["ElasticsearchSettings:username"], configuration["ElasticsearchSettings:password"])
                    }))
                    .WriteTo.Async(writeTo => writeTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"))
                    .CreateLogger();
        builder.Logging.ClearProviders();
        builder.Host.UseSerilog(Log.Logger, true);
        return builder;
    }
    public static WebApplication UseSerilog(this WebApplication app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseSerilogRequestLogging(opts =>
                {
                    opts.EnrichDiagnosticContext = LogEnricherExtensions.EnrichFromRequest;
                });
        return app;
    }
}

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.Forbidden;
        var result = string.Empty;

        if (exception?.Source == "Microsoft.AspNetCore.Authorization")
        {
            Log.Error(exception, "Error");
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }

        Log.Error(exception, "Error");
        code = HttpStatusCode.InternalServerError;
        result = System.Text.Json.JsonSerializer.Serialize(new { error = exception?.Message });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}


public static class LogEnricherExtensions
{
    public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        diagnosticContext.Set("UserName", httpContext?.GetUserName());
        diagnosticContext.Set("ClientIP", httpContext?.Connection?.RemoteIpAddress?.ToString());
        diagnosticContext.Set("UserAgent", httpContext?.Request?.Headers?["User-Agent"].FirstOrDefault());
        diagnosticContext.Set("Resource", httpContext?.GetMetricsCurrentResourceName());
    }

    private static string? GetUserName(this HttpContext httpContext)
    {
        var claimsIdentity = httpContext?.User?.Identity == null ? throw new NullReferenceException() : (ClaimsIdentity)httpContext.User.Identity;
        return claimsIdentity?.FindFirst(p => p.Type == "preferred_username")?.Value;
    }

    public static string? GetMetricsCurrentResourceName(this HttpContext httpContext)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));
        return httpContext.Request?.Path;
    }
}
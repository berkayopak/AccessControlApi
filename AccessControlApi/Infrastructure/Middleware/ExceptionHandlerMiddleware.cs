using System.Security.Authentication;
using System.Text.Json;
using AccessControlApi.Common;

namespace AccessControlApi.Infrastructure.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private const string ContentTypeJson = "application/json";
    private const string ApplicationException = "Application Error";
    private const bool ThrowExceptionInDevelopmentEnvironment = false;

    public ExceptionHandlerMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context, IHostEnvironment environment, ILoggerFactory loggerFactory)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
                throw;
            context.Response.Clear();
            await HandleExceptionAsync(context, exception, environment, loggerFactory);
        }
    }

    private static Task HandleExceptionAsync(
        HttpContext context,
        Exception ex,
        IHostEnvironment environment,
        ILoggerFactory loggerFactory)
    {
        //P.S: I want to see exception itself when I'm debugging on my local env, so I added below condition(environment.IsDevelopment()) for that.
        //Development => Local environment, Staging => Test environment
        //But I also added a parameter(ThrowExceptionInDevelopmentEnvironment),
        //because if you want to see the json result in the local environment,
        //you can set this parameter to false and see it.
        if (ThrowExceptionInDevelopmentEnvironment && environment.IsDevelopment())
            throw ex;

        var logger = loggerFactory.CreateLogger(ex.TargetSite?.DeclaringType ?? typeof(Exception));
        logger.LogError(ex, ex.Message);

        var jsonOpt = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        switch (ex)
        {
            case AuthenticationException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;

            case CustomApplicationException customAppEx:
                context.Response.StatusCode = customAppEx.StatusCode;
                context.Response.ContentType = ContentTypeJson;
                var customAppExResult = new OperationResult<object?>(null, false, customAppEx.Message, null, customAppEx.StatusCode);
                var serializedCustomAppExResult = JsonSerializer.Serialize(customAppExResult, jsonOpt);
                return context.Response.WriteAsync(serializedCustomAppExResult);

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = ContentTypeJson;
                var result = environment.IsStaging()
                    ? new OperationResult<object?>(null, false, ex.Message, null, 500)
                    : new OperationResult<object?>(null, false, ApplicationException, null, 500);

                var serializedResult = JsonSerializer.Serialize(result, jsonOpt);
                return context.Response.WriteAsync(serializedResult);
        }
    }
}
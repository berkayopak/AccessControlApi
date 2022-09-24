using AccessControlApi.Infrastructure.Extension;
using AccessControlApi.Infrastructure.Middleware;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Warn("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;
    var environment = builder.Environment;

    builder.Services.AddServices(environment, configuration);

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.WebHost.UseNLog();

    var app = builder.Build();

    app.MapOurHealthChecks();

    app.UseMiddleware<ExceptionHandlerMiddleware>();

    if (!environment.IsProduction())
    {
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UseCustomSwagger(apiVersionDescriptionProvider);
    }

    //P.S: I commented UseHttpsRedirection function to avoid dealing with sql certificate installations in local.
    //For staging or production environments, UseHttpsRedirection function should be added to code after the ssl certificate is set.
    //app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.UpdateDatabase();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}
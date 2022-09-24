using AccessControlApi.Helper;
using AccessControlApi.Repository;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;

namespace AccessControlApi.Infrastructure.Extension;

public static class AppBuilderExtension
{
    public static void UseCustomSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });
    }

    public static void MapOurHealthChecks(this WebApplication app)
    {
        //I could have added the UI for HealthCheck, but since it's best practice to host the UI in a different project,
        //I didn't go into that part to save some time.
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckHelper.WriteResponse,
            Predicate = healthCheck => healthCheck.Tags.Contains("ready")
        });

        app.MapHealthChecks("/health/detail", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckHelper.WriteResponse
        });
    }

    public static async void UpdateDatabase(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateAsyncScope())
        {
            var context = serviceScope?.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if(context is null)
                return;

            var anyPendingMigration = (await context.Database.GetPendingMigrationsAsync()).ToList();
            if(anyPendingMigration.Any())
                await context.Database.MigrateAsync();
        }
    }
}
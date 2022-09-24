using AccessControlApi.Config;
using AccessControlApi.Infrastructure.Swagger;
using AccessControlApi.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AccessControlApi.Domain;
using Microsoft.AspNetCore.Identity;
using AccessControlApi.Facade;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AccessControlApi.Infrastructure.Extension;

public static class ServiceCollectionExtension
{
    public static void AddServices(this IServiceCollection services, IHostEnvironment environment,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SmartLock");

        //Data Protection
        services.AddDataProtectionWithOptions();

        //HealthChecks
        services.AddOurHealthChecks(connectionString);

        //Config
        services.Configure<ApplicationConfig>(
            configuration.GetSection(ApplicationConfig.Application));

        //Identity
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        //Controllers
        services.AddControllers();

        //EfCore DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        //Auth
        services.AddAuthenticationWithOptions(configuration);
        services.AddAuthorization();

        //Documentation
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning();
        if(!environment.IsProduction())
            services.AddSwaggerWithOptions();

        //Facades
        services.AddTransient<IUserFacade, UserFacade>();
        services.AddTransient<IDoorFacade, DoorFacade>();

        //Repositories
        services.AddScoped<IDoorRepository, DoorRepository>();
    }

    private static void AddDataProtectionWithOptions(this IServiceCollection services)
    {
        services.AddDataProtection()
            .UseCryptographicAlgorithms(
                new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });
    }

    private static void AddOurHealthChecks(this IServiceCollection services, string connectionString)
    {
        //Added "ready" tag for deployment platforms/tools (for example: kubernetes)
        services.AddHealthChecks()
            .AddCheck("access-control-api", () => HealthCheckResult.Healthy("Live"), new[] { "ready" })
            .AddSqlServer(connectionString);
    }

    private static void AddAuthenticationWithOptions(this IServiceCollection services, IConfiguration configurationManager)
    {
        //P.S: Since this is just a project I run locally,
        //I set the Jwt's "ValidateIssuer" and "ValidateAudience" setting variables to false.
        //However, if we need to deploy this project to the server,
        //we need to enter the "ValidIssuer" and "ValidAudience"
        //information correctly after setting these setting variables to true.
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(configurationManager["Application:Auth:Secret"])),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            };
        });
    }

    private static void AddApiVersioning(this IServiceCollection services)
    {
        //P.S: Especially in mobile clients, when the request or response content of a function changes, corruption may occur on client-side.
        //That's why we add versioning to our api endpoints. And thus, we get a more stable API client interaction.
        services.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            o.ReportApiVersions = true;
            o.ApiVersionReader = new HeaderApiVersionReader("X-Version");
        });

        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });
    }

    private static void AddSwaggerWithOptions(this IServiceCollection services)
    {
        var securityScheme = new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JSON Web Token based security",
        };

        var securityReq = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        };

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", securityScheme);
            options.AddSecurityRequirement(securityReq);
        });
        services.ConfigureOptions<ConfigureSwaggerOptions>();
    }
}
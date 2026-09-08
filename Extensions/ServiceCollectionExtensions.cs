using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

namespace BackendTZ.Extensions;

public static class ServiceCollectionExtensions
{
    private const string BearerSchemeId = "Bearer";

    /// <summary>
    /// Registers JWT Bearer authentication and authorization.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var signingKey = config["AppSettings:AccessToken"]
                         ?? throw new InvalidOperationException("AppSettings:AccessToken не налаштовано.");

        services.AddAuthorization();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = config["AppSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["AppSettings:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ValidateIssuerSigningKey = true
                };
            });

        return services;
    }

    /// <summary>
    /// Registers Swagger with support for JWT authorization and XML comments.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Conference Room Booking API",
                Version = "v1",
                Description = "API for managing conference rooms, reservations and calculating rental costs"
            });

            c.AddSecurityDefinition(BearerSchemeId, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter the JWT token in the format: Bearer {token}"
            });

            c.AddSecurityRequirement(document =>
            {
                var schemeRef = new OpenApiSecuritySchemeReference(BearerSchemeId, document);
                return new OpenApiSecurityRequirement
                {
                    [schemeRef] = new List<string>()
                };
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    /// <summary>
    /// Registers CORS policy based on allowed origins from configuration.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration config)
    {
        var allowedOrigins = config
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>()
            ?.Select(o => o.TrimEnd('/'))
            .ToArray();

        if (allowedOrigins is null || allowedOrigins.Length == 0 || allowedOrigins.Any(o => o == "*"))
        {
            throw new InvalidOperationException("CORS: incorrect configuration of AllowedOrigins.");
        }

        services.AddCors(options =>
        {
            options.AddPolicy("AllowConfiguredOrigins", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .WithHeaders("Content-Type", "Authorization");
            });
        });

        return services;
    }
}
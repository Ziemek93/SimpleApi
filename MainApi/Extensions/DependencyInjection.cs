using System.Security.Cryptography;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MainApi.Models.Comments;
using MainApi.Options;
using MainApi.Repositories.Articles;
using MainApi.Repositories.User;
using MainApi.Services;
using MainApi.Services.ArticleService;
using MainApi.Services.UserService;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MainApi.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureBasicApiServices(this IServiceCollection services)
    {
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }


    public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("M2MClient").Get<JwtOptions>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config.Issuer,
                    ValidAudience = config.Audience,
                    IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                    {
                        var jwksService = services.BuildServiceProvider().GetRequiredService<JwksService>();
                        return jwksService.GetSigningKeysAsync().Result;
                    }
                };
            });
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));

            options.AddPolicy("IsUser", policy =>
                policy.RequireAuthenticatedUser().RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == "role")));

            options.AddPolicy("CanWriteInteractions", policy =>
                policy.RequireClaim("scope", "write:user-interactions"));
        });
        services.AddFastEndpoints();

        return services;
    }

    public static IServiceCollection ConfigureMassTransit(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RabbitMq");
        var sender = configuration.GetSection("MessageSender");

        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped); // Możesz jawnie określić ServiceLifetime


        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(connectionString);

                cfg.Message<CreateCommentRequest>(x => { x.SetEntityName(sender["CommentsExchange:Exchange"]); });

                cfg.Publish<CreateCommentRequest>(x => { x.ExchangeType = sender["CommentsExchange:Type"]; });

                cfg.Message<CreateChatMessageRequest>(x =>
                {
                    x.SetEntityName(sender["ChatMessagesExchange:Exchange"]);
                });

                cfg.Publish<CreateChatMessageRequest>(x => { x.ExchangeType = sender["ChatMessagesExchange:Type"]; });


                cfg.UseRawJsonSerializer();

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static void ApplyPendingMigrations<TDbContext>(this IApplicationBuilder app)
        where TDbContext : DbContext
    {
        using var serviceScope = app.ApplicationServices.CreateScope();

        var appName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;

        var loggerFactory = serviceScope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("MigrationExtensions");

        try
        {
            logger.LogInformation("{appName}: Downloading dbcontext type {DbContextType}...", typeof(TDbContext).Name,
                appName);
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();

            logger.LogInformation("{appName}: Checking migrations", appName);
            dbContext.Database.Migrate();

            logger.LogInformation("{appName}: migration completed.", appName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{appName}: Error occured durning migration.", appName);
            throw;
        }
    }
}
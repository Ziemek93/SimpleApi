using MainApi.Repositories.Articles;
using MainApi.Repositories.User;
using MainApi.Services;
using MainApi.Services.ArticleService;
using MainApi.Services.UserService;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Broker.Contracts;

namespace MainApi;

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

    public static IServiceCollection ConfigureMassTransit(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RabbitMq");
        var sender = configuration.GetSection("MessageSender");
        
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(connectionString);

                cfg.Message<CreateCommentContract>(x =>
                {
                    x.SetEntityName(sender["CommentsExchange:Exchange"]);
                });

                cfg.Publish<CreateCommentContract>(x =>
                {
                    x.ExchangeType = sender["CommentsExchange:Type"];
                });

                cfg.Message<CreateChatMessageContract>(x =>
                {
                    x.SetEntityName(sender["ChatMessagesExchange:Exchange"]);
                });

                cfg.Publish<CreateChatMessageContract>(x =>
                {
                    x.ExchangeType = sender["ChatMessagesExchange:Type"];
                });

                cfg.UseRawJsonSerializer();

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static async Task ApplyPendingMigrations<TDbContext>(this IApplicationBuilder app, bool isDev = false)
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

            if (isDev)
            {
                logger.LogInformation("{appName}: Dropping database...", appName);
                var deleted = await dbContext.Database.EnsureDeletedAsync();
                logger.LogInformation("{appName}: Database deleted: {deleted}", appName, deleted);
            }
            
            logger.LogInformation("{appName}: Checking migrations", appName);
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("{appName}: Migration completed.", appName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{appName}: Error occured durning migration.", appName);
            throw;
        }
    }

    public static async Task SeedDemoData(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var seeder = services.GetRequiredService<DatabaseSeeder>();
                await seeder.SeedAsync();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured durning demo seed.");
            }
        }
    }
}
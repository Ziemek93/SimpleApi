using System.Reflection;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using UsersInteractions.Application.Abstractions;
using UsersInteractions.Application.Data;
using UsersInteractions.Infrastructure.Consumers;
using UsersInteractions.Infrastructure.Data;
using UsersInteractions.Infrastructure.Middleware;
using UsersInteractions.Infrastructure.Options;
using UsersInteractions.Infrastructure.Services;

namespace UsersInteractions.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("InteractionsDbConnection");
        var redisConnectionString = configuration.GetConnectionString("Redis");

        // postgres setup 
        services.AddDbContext<ApplicationContext>((sp, options) => { options.UseNpgsql(connectionString); });

        services.AddScoped<IApplicationContext, ApplicationContext>();

        // redis setup
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
        services.AddScoped<ICacheService, RedisCacheService>();

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
            dbContext.Database.Migrate();

            logger.LogInformation("{appName}: migration completed.", appName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{appName}: Error occured durning migration.", appName);
            throw;
        }
    }

    public static IServiceCollection ConfigureMassTransit(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RabbitMq");
        var commentsOptions = configuration.GetSection("MessageConsumers:Comments").Get<MessageConsumerOptions>();
        var chatOptions = configuration.GetSection("MessageConsumers:ChatMessages").Get<MessageConsumerOptions>();

        // var consumerConfigs = configuration.GetSection("MessageConsumers")();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMassTransit(x =>
        {
            x.AddConsumer<CommentReceivedEventHandler>();
            x.AddConsumer<MessageReceivedEventHandler>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(connectionString);
                
                // cfg.Message<UsersInteractions.Infrastructure.Events.CreateCommentRequest>(m =>
                // {
                //     m.SetEntityName(commentsOptions.Exchange);
                // });
                //
                // cfg.Message<UsersInteractions.Infrastructure.Events.CreateChatMessageRequest>(m =>
                // {
                //     m.SetEntityName(chatOptions.Exchange);
                // });

                cfg.ReceiveEndpoint(commentsOptions.Queue, e =>
                {
                    e.Bind(commentsOptions.Exchange, x =>
                    {
                        x.ExchangeType = commentsOptions.ExchangeType;
                        x.RoutingKey = commentsOptions.RoutingKey;
                    });

                    e.ConfigureConsumer<CommentReceivedEventHandler>(context);
                    e.UseConsumeFilter(typeof(ValidationFilter<>), context);
                });

                cfg.ReceiveEndpoint(chatOptions.Queue, e =>
                {
                    e.Bind(chatOptions.Exchange, x =>
                    {
                        x.ExchangeType = chatOptions.ExchangeType;
                        x.RoutingKey = chatOptions.RoutingKey;
                    });

                    e.ConfigureConsumer<MessageReceivedEventHandler>(context);
                    e.UseConsumeFilter(typeof(ValidationFilter<>), context);
                });

                cfg.UseRawJsonSerializer();
            });
        });

        return services;
    }
}
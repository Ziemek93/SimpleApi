﻿using AuthApi.Data;
using AuthApi.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AuthConfiguration(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser , IdentityRole>(x =>
        {
            x.Password.RequireDigit = true;
            x.Password.RequireLowercase = true;
            x.Password.RequiredLength = 8;
            x.Password.RequireNonAlphanumeric = false;
            x.Password.RequireUppercase = false;
            x.Password.RequireLowercase = false;
        })
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddDefaultTokenProviders();

        return services;
    }
    
    public static async Task ApplyPendingMigrations<TDbContext>(this WebApplication app, bool isDev = false) 
        where TDbContext : IdentityDbContext<ApplicationUser>
    {
        // using var serviceScope = app.ApplicationServices.CreateScope();
        using var scope = app.Services.CreateScope();
     
        var appName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;
        
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("MigrationExtensions");

        try
        {
            logger.LogInformation("{appName}: Downloading dbcontext type {DbContextType}...", typeof(TDbContext).Name, appName);
            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

            if (isDev)
            {
                logger.LogInformation("{appName}: Dropping database...", appName);
                var deleted = await dbContext.Database.EnsureDeletedAsync();
                logger.LogInformation("{appName}: Database deleted: {deleted}", appName, deleted);
            }
            
            logger.LogInformation("{appName}: Checking migrations", appName);
            await dbContext.Database.MigrateAsync();

            logger.LogInformation("{appName}: migration completed.", appName, ConsoleColor.Blue);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{appName}: Error occured durning migration.", appName);
            throw;
        }
    }
}

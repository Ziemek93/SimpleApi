using Flurl.Http.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Auth.Options;
using Shared.Auth.OptionsConfigurations;
using Shared.Auth.Services;

namespace Shared.Auth;

public static class DependencyInjection
{
    
    public static IServiceCollection ConfigureAppAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IFlurlClientCache, FlurlClientCache>();
        services.Configure<PublicKeyOptions>(configuration.GetSection("PublicKey"));
        services.AddMemoryCache();
        services.AddSingleton<JwksService>();
        services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, JwtOptionsConfiguration>();

        var jwtSettings = configuration.GetSection("Jwt").Get<JwtOptions>();
        var m2mClientSettings = configuration.GetSection("M2MClients").Get<M2MOptions>();
        
        if (jwtSettings is null || string.IsNullOrEmpty(jwtSettings.Audience) || string.IsNullOrEmpty(jwtSettings.Issuer))
        {
            throw new InvalidOperationException("Jwt options are required in appsettings.json");
        }
        
        var validAudiences = new List<string> { jwtSettings.Audience };
        
        if (m2mClientSettings is not null && !string.IsNullOrEmpty(m2mClientSettings.Audience))
        {
            validAudiences.Add(m2mClientSettings.Audience);
        }
        
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                x.MapInboundClaims = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, 
                    ValidateAudience = true, 
                    ValidateLifetime = true, 
                    ValidateIssuerSigningKey = true, 
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudiences = validAudiences,
                    ClockSkew = TimeSpan.FromSeconds(5)
                };
            });
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));

            options.AddPolicy("IsUser", policy =>
                policy.RequireAuthenticatedUser().RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == "role")));
            if(!string.IsNullOrEmpty(m2mClientSettings?.AllowedScopes))
            {
                options.AddPolicy("CanWriteInteractions", policy =>
                    policy.RequireClaim("scope", m2mClientSettings.AllowedScopes));
            }
        });

        return services;
    }
    
    public static IApplicationBuilder UseAppAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
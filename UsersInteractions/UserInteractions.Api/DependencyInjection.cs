using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UsersInteractions.Domain.Options;
using UsersInteractions.Infrastructure.Services;

namespace UserInteractions.Api;

public static class DependencyInjection
{
       public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt").Get<JwtOptions>();
            var m2mClientSettings = configuration.GetSection("M2MClients").Get<M2MOptions>();

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
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudiences = new [] {jwtSettings.Audience, m2mClientSettings.Audience},
                    ClockSkew = TimeSpan.FromSeconds(5),
                    IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                    {
                        var jwksService = services.BuildServiceProvider().GetRequiredService<JwksService>();
                        return jwksService.GetSigningKeysAsync().Result;
                    }
                    
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsUser", policy =>
                    policy.RequireAuthenticatedUser().RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == "role")));

                options.AddPolicy("CanWriteInteractions", policy =>
                    policy.RequireClaim("scope", m2mClientSettings.AllowedScopes));
            });

            return services;
        }
}
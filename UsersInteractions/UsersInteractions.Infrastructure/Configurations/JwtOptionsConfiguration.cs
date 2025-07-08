using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Shared.Auth.Services;

namespace UsersInteractions.Infrastructure.Configurations;

public class JwtOptionsConfiguration : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly JwksService _jwksService;

    public JwtOptionsConfiguration(JwksService jwksService)
    {
        _jwksService = jwksService;
    }

    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/messageHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    }
}

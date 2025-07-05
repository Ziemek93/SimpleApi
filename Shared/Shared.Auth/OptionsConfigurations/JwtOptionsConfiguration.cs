using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Shared.Auth.Services;

namespace Shared.Auth.OptionsConfigurations;

public class JwtOptionsConfiguration : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly JwksService _jwksService;

    public JwtOptionsConfiguration(JwksService jwksService)
    {
        _jwksService = jwksService;
    }

    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.TokenValidationParameters.IssuerSigningKeyResolver = (token, securityToken, kid, parameters) 
            => _jwksService.GetSigningKeysAsync().GetAwaiter().GetResult();
    }
}
using FastEndpoints;
using Flurl.Http;
using Flurl.Http.Configuration;
using MainApi.Models;
using MainApi.Options;
using Microsoft.Extensions.Options;

namespace MainApi.Endpoints.Public.Auth.Login;

public class Endpoint : Endpoint<Request, Response>
{


    private readonly AuthApiOptions _authApiOptions;
    private readonly IFlurlClient _flurlClient;

    public Endpoint(IOptions<AuthApiOptions> authApiOptions, IFlurlClientCache flurlClient)
    {
        _authApiOptions = authApiOptions.Value;
        _flurlClient = flurlClient.GetOrAdd(_authApiOptions.Base, _authApiOptions.Base);
    }

    public override void Configure()
    {
        Post("/api/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {

        var path = $"{_authApiOptions.Base}{_authApiOptions.Paths.Login}";


        var flurlRequest = await _flurlClient
            .Request(path)
            .WithHeader("Accept", "application/json")
            .WithHeader("Content-Type", "application/json")
            .WithTimeout(30)
            .PostJsonAsync(request, cancellationToken: ct);

        var token = await flurlRequest.GetJsonAsync<TokenResponse>();

        if (string.IsNullOrWhiteSpace(token.Token))
        {
            throw new BadHttpRequestException("Wrong username or password");
        }

        var response = new Response { Token = token.Token };

        await SendAsync(
            response, cancellation: ct
        );
    }
}
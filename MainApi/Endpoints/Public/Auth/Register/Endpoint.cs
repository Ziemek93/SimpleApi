using FastEndpoints;
using Flurl.Http;
using Flurl.Http.Configuration;
using MainApi.Context;
using MainApi.Models.Auth;
using MainApi.Models.Entities;
using MainApi.Options;
using Microsoft.Extensions.Options;

namespace MainApi.Endpoints.Public.Auth.Register;


public class Endpoint : Endpoint<Request, Response>
{
    private IConfiguration _config { get; set; }
    public ApplicationContext _context { get; set; }
    private readonly AuthApiOptions _authApiOptions;
    private readonly IFlurlClient _flurlClient;

    //private static readonly IFlurlClient _flurlClient;

    public Endpoint(IConfiguration config, ApplicationContext context, IOptions<AuthApiOptions> authApiOptions, IFlurlClientCache flurlClient)
    {
        _config = config;
        _context = context;
        _authApiOptions = authApiOptions.Value;
        _flurlClient = flurlClient.GetOrAdd(_authApiOptions.Base, _authApiOptions.Base);
    }

    public override void Configure()
    {
        Post("/api/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var path = $"{_authApiOptions.Base}{_authApiOptions.Paths.Register}";


        var flurlRequest = await _flurlClient
            .Request(path)
            .WithHeader("Accept", "application/json")
            .WithHeader("Content-Type", "application/json")
            .WithTimeout(30)
            .PostJsonAsync(request, cancellationToken: ct);

        if (flurlRequest?.StatusCode != 200)
            ThrowError("User creation failed");

        var flurResponse = await flurlRequest.GetJsonAsync<UserDto>();

        _context.Users.Add(new User
        {
            UserId = flurResponse.Id,
            UserName = flurResponse.Username
        });
        await _context.SaveChangesAsync();
        
        Response.Message = $"The user [{request.Username}] has been created.";

        await SendAsync(Response, cancellation: ct);
    }

}


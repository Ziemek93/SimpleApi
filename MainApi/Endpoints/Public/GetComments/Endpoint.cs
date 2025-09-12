using FastEndpoints;
using Flurl.Http;
using Flurl.Http.Configuration;
using MainApi.Interfaces;
using MainApi.Models.Comments;
using MainApi.Options;
using MainApi.Services.ArticleService;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace MainApi.Endpoints.Public.GetComments;

public class Endpoint : EndpointWithoutRequest<Response>
{
    private readonly InteractionsApiOptions _interactionsApiOptions;
    private readonly IFlurlClient _flurlClient;
    private readonly IM2MTokenService _tokenService;

    public Endpoint(IM2MTokenService tokenService, IOptions<InteractionsApiOptions> interactionsApiOptions, IFlurlClientCache flurlClient)
    {
        _tokenService = tokenService;
        _interactionsApiOptions = interactionsApiOptions.Value;
        _flurlClient = flurlClient.GetOrAdd(_interactionsApiOptions.Base, _interactionsApiOptions.Base);
    }
    
    public override void Configure()
    {
        Get("/api/article/{id}/comments");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken token = default)
    {
        // var baseUrl = configuration["InteractionsApi:Base"];
        // var commentsPath = configuration["InteractionsApi:Paths:Comments"];
        //
        // var postId = 123;
        // var url = baseUrl + string.Format(commentsPath, postId);
        var articleID = Route<int>("Id").ToString();
        var addr = string.Format(_interactionsApiOptions.Paths.Comments, articleID);
        var path = 
            $"{_interactionsApiOptions.Base}{addr}";
        var authTokenJson = await _tokenService.GetTokenAsync(token);
        JObject jsonObject = JObject.Parse(authTokenJson);
        var authToken = jsonObject["m2m"]?.ToString();
        // string token = jsonObject["m2m"]?.ToString();
        
        var flurlRequest = await _flurlClient
            .Request(path)
            .WithHeader("Accept", "application/json")
            .WithHeader("Content-Type", "application/json")
            .WithHeader("Authorization", $"Bearer {authToken}")
            .WithTimeout(30)
            .GetAsync(cancellationToken: token);

        var response = await flurlRequest.GetJsonAsync<Response>();
        // if ()
        // {
        //     AddError(result.Errors.First());
        //     await SendNotFoundAsync(token);
        //     return;
        // }
        await SendOkAsync(response, token);
    }
}
